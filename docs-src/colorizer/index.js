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

let names = {
    "csharp": "C#",
    "html": "HTML",
    "js": "Javascript",
    "cshtml": "Razor",
    "powershell": "Powershell",
    "bash":  "Bash",
    "json": "JSON",
    "rest": "REST",
    "xml": "XML"
}

async function main() {
    // edit docfx.js too! line 84
    let grammars = {
        "csharp": await registry.loadGrammar('source.cs'),
        "html": await registry.loadGrammar('text.html.basic'),
        "js": await registry.loadGrammar('source.js'),
        "cshtml": await registry.loadGrammar("text.html.cshtml"),
        "powershell": await registry.loadGrammar("source.powershell"),
        "bash":  await registry.loadGrammar("source.shell"),
        "json":  await registry.loadGrammar("source.json"),
        "rest": await registry.loadGrammar("source.rest"),
        "xml": await registry.loadGrammar("text.xml")
    };

    let root = path.resolve(__dirname, "../../docs");
    let files = getAllFiles(root).filter(f => f.endsWith(".html"));
    let finishedFiles = [];
    files.forEach(async f => {
        await updateFile(f, grammars);
        finishedFiles.push(f);
    });
    let donePromise = new Promise(async resolve => {
        while (finishedFiles.length != files.length) { await delay(100); }
        resolve();
    });

    await Promise.race([donePromise, delay(30000)]);
    if (finishedFiles.length != files.length) {
        console.log("The following files timed out", files.filter(f => finishedFiles.indexOf(f) === -1));
    }

    process.exit(0);
}

async function delay(ms) {
    return new Promise(resolve => {setTimeout(resolve, ms);});
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

            if (colorized.class) node.setAttribute("class", colorized.class);

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
    let genericMatch = raw.match(/^<code\s*>/);
    if (!startMatch && !genericMatch) return null;
    let lang = startMatch ? startMatch[1] : "csharp";
    let grammar = grammars[lang]
    if (!grammar) return null;
    raw = raw.substring(startMatch ? startMatch[0].length : genericMatch[0].length);
    if (raw.endsWith('</code>')) raw = raw.substring(0, raw.length - 7);
    const text = raw.split('\n').map(l => l.replace(/\r/g, ""));
    const highlightLines = (startMatch && startMatch[4]) ? startMatch[4].split(",").map(range => {
        let rangeParts = range.split("-").map(p => parseInt(p));
        return { start: rangeParts[0], end: rangeParts[rangeParts.length - 1] };
    }) : [];

    // header
    let headerMatch = text.length > 0 && text[0].match(/^(\/\/\s)?---\sHeader: (.*)\s(nocopy)?---$/i);
    let header = null;
    let id = null;
    let preClass = "";
    if (true /*headerMatch*/) { // every block should have a header now that I think about it
        id = randomString(8);
        let headerText = headerMatch ? headerMatch[2] : names[lang];
        if (headerText === "Request") preClass = "request-pre";

        let headerClass = "code-header";
        if (headerText === "Response") headerClass += " response-header";
        header = new htmlParser.HTMLElement("div", { class: headerClass });
        let textContainer = new htmlParser.HTMLElement("span", {class: "language"});
        textContainer.appendChild(new htmlParser.TextNode(headerText));
        header.appendChild(textContainer)

        if (!headerMatch || !headerMatch[3]){ // nocopy not specified
            let spacer = new htmlParser.HTMLElement("div", {class: "spacer"});
            let copyBtn = new htmlParser.HTMLElement("button", {class: "copy-btn"});
            copyBtn.setAttribute("onclick", `copyCode(this,'${id}')`);
            copyBtn.appendChild(new htmlParser.TextNode("Copy"));
            header.appendChild(spacer);
            header.appendChild(copyBtn);
        }

        if (headerMatch)
            text.splice(0, 1); // remove header
    }

    if (genericMatch) {
        // trim whitespace
        let shortestWhitespace = Number.MAX_VALUE;
        for (let i = 0; i < text.length; i++) {
            let whitespace = text[i].match(/^\s*/)[0].length;
            if (whitespace < shortestWhitespace) shortestWhitespace = whitespace;
        }

        for (let i = 0; i < text.length; i++)  
            text[i] = text[i].substring(shortestWhitespace);
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
                let settingsList = token.scopes.reverse().map(s => matchScope(s)).filter(s => !!s);
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

    return { text: new htmlParser.TextNode(`<code>${output}</code>`), header, id, class: preClass };
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