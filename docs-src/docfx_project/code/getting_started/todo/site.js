// <main>
// --- Header: site.js ---
async function getTodos() {
	let response = await fetch("/todos");
	if (response.status !== 200) throw new Error(response.text());
	let todos = await response.json();
	document.getElementById("todo-list").innerHTML = "";
	todos.forEach(t => {
		document.getElementById("todo-list").appendChild(createTodoItem(t))
	});
}

function createTodo(text) {
	return fetch("/todos", {
		body: JSON.stringify({ Text: text }),
		headers: { "Content-Type": "application/json" },
		method: "POST"
	});
}

function deleteTodo(id) {
	return fetch(`/todos/${id}`, {
		method: "DELETE"
	}).then(getTodos);
}

async function updateTodo(id, text) {
	return fetch(`/todos/${id}`, {
		method: "PATCH",
		body: JSON.stringify({ Text: text }),
		headers: { "Content-Type": "application/json" }
	});
}

function onKeyDown(id, e) {
	if (e.keyCode == 13) updateTodo(id, this.value).then(r => {
		this.style.transition = "background-color 150ms ease";
		this.style.backgroundColor = r.status == 200 ? "#bae0ff" : "#ffc3be";
		setTimeout(() => { this.style.backgroundColor = ""; setTimeout(getTodos, 75); }, 75);
	});
}

function createTodoItem(todo) {
	let newEl = (name, attrs) => { let el = document.createElement(name); for (key in attrs) el[key] = attrs[key]; return el };
	let root = newEl("li", { className: "list-group-item" });
	let inputGroup = newEl("div", { className: "input-group"});
	let prepend = newEl("div", { className: "input-group-prepend" });
	let idText = newEl("span", { className: "input-group-text", title: new Date(todo.Created).toLocaleString() });
	idText.appendChild(document.createTextNode(todo.Id));
	let input = newEl("input", {type: "text", className: "form-control", value: todo.Text });
	input.onkeyup = onKeyDown.bind(input, todo.Id);
	let append = newEl("div", { className: "input-group-append" })
	let deleteBtn = newEl("div", { className: "btn btn-danger", innerHTML: "\u00D7", onclick: deleteTodo.bind(null, todo.Id) })

	root.appendChild(inputGroup);
	inputGroup.appendChild(prepend);
	prepend.appendChild(idText);
	inputGroup.appendChild(input);
	append.appendChild(deleteBtn);
	inputGroup.appendChild(append);
	return root;
}

document.getElementById("create-btn").addEventListener("click", () => {
	let input = document.getElementById("new-todo");
	createTodo(input.value).then(async r => r.status == 200 || alert("Couldn't create todo: " + await r.text())).then(getTodos);
	input.value = "";
});
getTodos().catch(e => alert("Couldn't GET /todos:" + e));
// </main>

// <random>
// --- Header: site.js ---
// add to the end of the file
fetch("/todos/random").then(r => r.json().then(todos => 
	document.getElementById("random-todo").appendChild(document.createTextNode(todos[0].Text))
));
// </random>