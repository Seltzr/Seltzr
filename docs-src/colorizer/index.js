const fs = require('fs');
const textmate = require("vscode-textmate");
const oniguruma = require("oniguruma");
const htmlParser = require('node-html-parser');
const escapeHtml = require('escape-html');
const colorTokens = require('./colorTokens');
const path = require('path');

function readFile(path) {
    return new Promise((resolve, reject) => {
        fs.readFile(path, (error, data) => error ? reject(error) : resolve(data));
    })
}

function writeFile(path, contents) {
    return new Promise((resolve, reject) => {
        fs.writeFile(path, contents, (error) => error ? reject(error) : resolve());
    })
}

// Create a registry that can create a grammar from a scope name.
const registry = new textmate.Registry({
    onigLib: Promise.resolve({
        createOnigScanner: (sources) => new oniguruma.OnigScanner(sources),
        createOnigString: (str) => new oniguruma.OnigString(str)
    }),
    loadGrammar: (scopeName) => {
        return readFile(`./${scopeName}.tmLanguage.json`).then(data => {
            let grammar = textmate.parseRawGrammar(data.toString(), ".json");
            return grammar;
        });
    }
});

async function main() {
    // edit docfx.js too! line 84
    let grammars = {
        "csharp": await registry.loadGrammar('source.cs'),
        "html": await registry.loadGrammar('text.html.basic'),
        "js": await registry.loadGrammar('source.js'),
        "cshtml": await registry.loadGrammar("text.html.cshtml"),
        "powershell": await registry.loadGrammar("source.powershell"),
        "bash":  await registry.loadGrammar("source.shell")
    };

    let root = path.resolve(__dirname, "../../docs");
    let files = getAllFiles(root).filter(f => f.endsWith(".html"));
    files.forEach(f => updateFile(f, grammars));
}

main();

async function updateFile(path, grammars) {
    let contents = (await readFile(path)).toString();
    let rootNode = htmlParser.parse(contents, { pre: true });
    let codeBlocks = rootNode.querySelectorAll("pre");
    let updated = 0;
    for (let i = 0; i < codeBlocks.length; i++) {
        let node = codeBlocks[i];
        let colorized = colorizeNode(node.text, grammars);
        if (colorized){
            let parentIndex = node.parentNode.childNodes.indexOf(node);
            if (colorized.header) { // header
                node.parentNode.childNodes.splice(parentIndex, 0, colorized.header);
                node.setAttribute("id", colorized.id);
            }
            node.set_content(colorized.text);
            updated++;
        }
    }
    if (updated === 0) return;
    
    let result = rootNode.toString();
    await writeFile(path, result);
    console.log(`Updated ${updated} codeblocks in ${path}`);
}
function colorizeNode(raw, grammars) {
    let startMatch = raw.match(/^<code class="lang-(\w+)"(.*?)(highlight-lines="(.*?)"(.*?))?>/);
    if (!startMatch) return null;
    let grammar = grammars[startMatch[1]]
    if (!grammar) return null;
    raw = raw.substring(startMatch[0].length);
    if (raw.endsWith('</code>')) raw = raw.substring(0, raw.length - 7);
    const text = raw.split('\n').map(l => l.replace(/\r/g, ""));
    const highlightLines = startMatch[4] ? startMatch[4].split(",").map(range => {
        let rangeParts = range.split("-").map(p => parseInt(p));
        return { start: rangeParts[0], end: rangeParts[rangeParts.length - 1] };
    }) : [];

    // header
    let headerMatch = text.length > 0 && text[0].match(/^(\/\/\s)?---\sHeader: (.*)\s(nocopy)?---$/i);
    let header = null;
    let id = null;
    if (headerMatch) {
        id = randomString(8);
        let headerText = headerMatch[2];
        header = new htmlParser.HTMLElement("div", { class: "code-header" });
        let textContainer = new htmlParser.HTMLElement("span", {class: "language"});
        textContainer.appendChild(new htmlParser.TextNode(headerText));
        header.appendChild(textContainer)

        if (!headerMatch[3]){ // nocopy not specified
            let spacer = new htmlParser.HTMLElement("div", {class: "spacer"});
            let copyBtn = new htmlParser.HTMLElement("button", {class: "copy-btn"});
            copyBtn.setAttribute("onclick", `copyCode(this,'${id}')`);
            copyBtn.appendChild(new htmlParser.TextNode("Copy"));
            header.appendChild(spacer);
            header.appendChild(copyBtn);
        }

        text.splice(0, 1); // remove header
    }

    let output = "";
    let ruleStack = textmate.INITIAL;
    for (let i = 0; i < text.length; i++) {
        const line = text[i];
        const lineTokens = grammar.tokenizeLine(line, ruleStack);
        const lineNumber = i + 1;
        const doHighlight = highlightLines.some(r => r.start <= lineNumber && lineNumber <= r.end);
        if (doHighlight)
            output += '<span class="line-highlight">';

        for (let j = 0; j < lineTokens.tokens.length; j++) {
            const token = lineTokens.tokens[j];
            let sub = line.substring(token.startIndex, token.endIndex);
            if (token.scopes.length == 0) output += sub;
            else {
                let settingsList = token.scopes.map(s => matchScope(s)).filter(s => !!s);
                let settings = settingsList.length > 0 ? settingsList[0] : { foreground: "black" };
                if (!settings.foreground) settings.foreground = 'black';

                let style =  `color:${settings.foreground};`;
                if (settings.fontStyle) {
                    if (settings.fontStyle === 'bold') style += 'font-weight:bold;';
                    else if (settings.fontStyle === 'italic') style += 'font-style:italic';
                    else if (settings.fontStyle === 'underline') style += 'text-decoration:underline';
                }
                output += `<span style='${style}'>${escapeHtml(sub)}</span>`;
            }
        }
        
        if (doHighlight)
            output += '</span>';
        if (i < (text.length - 1))
            output += doHighlight ? '<span class="newline">\n</span>' : '\n';
        ruleStack = lineTokens.ruleStack;
    }

    return { text: new htmlParser.TextNode(`<code>${output}</code>`), header, id };
}

function randomString(length) {
    var randomChar = function () { return String.fromCharCode(Math.floor((Math.random() * (123 - 97)) + 97)); };
    var str = "";
    for (var i = 0; i < length; i++)
        str += randomChar();
    return str;
}

function matchScope(scope) {
    let parts = scope.split(".");

    while (parts.length > 0) {
        let match = matchScopeExact(parts.join("."));
        if (match) return match;
        parts.pop();
    }

    return null;
}

function matchScopeExact(scope) {
    for (let i = 0; i < colorTokens.length; i++) {
        let tokenProps = colorTokens[i];
        if (typeof tokenProps.scope == "string") {
            if (tokenProps.scope === scope) return tokenProps.settings;
            else continue;
        }

        if (tokenProps.scope.some(s => s === scope)) return tokenProps.settings;
    }

    return null;
}

function getAllFiles(directory) {
    let baseFiles = fs.readdirSync(directory);
    let trueFiles = [];

    baseFiles.forEach(fileName => {
        let absolutePath = path.resolve(directory, fileName);
        if (fs.statSync(absolutePath).isDirectory())
            trueFiles = trueFiles.concat(getAllFiles(absolutePath));
        else
            trueFiles.push(absolutePath)
    })

    return trueFiles;
}