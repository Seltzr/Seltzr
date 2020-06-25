// <efcore>
// --- Header: Startup.cs ---
public void ConfigureServices(IServiceCollection services) {
   services.AddRazorPages();
   services.AddDbContext<TodoContext>();
}
// </efcore>
// <step0>
// --- Header: Startup.cs ---
using Seltzr.Extensions;
// </step0>
// <step1>
// --- Header: Startup.cs ---
public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
    ...
    app.AddEFCoreSeltzr<Todo, TodoContext>("todos", api => { });
}
// </step1>
// <step2>
// --- Header: Startup.cs ---
app.AddEFCoreSeltzr<Todo, TodoContext>("todos", api => {
    api
        .ParseJson()
        .WriteJson()
        .CatchExceptions();
});
// </step2>
// <step3>
// --- Header: Startup.cs ---
app.AddEFCoreSeltzr<Todo, TodoContext>("todos", api => {
    api
        .ParseJson()
        .WriteJson()
        .CatchExceptions()
        .Get()
        .DeleteByPrimaryKey()
        .PatchUpdateByPrimaryKey()
        .PostCreate();
});
// </step3>
// <step4>
// --- Header: Startup.cs ---
app.AddEFCoreSeltzr<Todo, TodoContext>("todos", api => {
    api
        .ParseJson()
        .WriteJson()
        .CatchExceptions()
        .OrderBy(t => t.Text)
        .Get()
        .DeleteByPrimaryKey()
        .PatchUpdateByPrimaryKey()
        .PostCreate();
});
// </step4>
// <step5>
// --- Header: Startup.cs ---
app.AddEFCoreSeltzr<Todo, TodoContext>("todos", api => {
    api
        .ParseJson()
        .WriteJson()
        .CatchExceptions()
        .OrderBy(t => t.Text)
        .RequireAllInput(t => !string.IsNullOrWhiteSpace(t.Text))
        .Get()
        .DeleteByPrimaryKey()
        .PatchUpdateByPrimaryKey()
        .PostCreate();
});
// </step5>
// <step6>
// --- Header: Startup.cs ---
app.AddEFCoreSeltzr<Todo, TodoContext>("todos", api => {
    api
        .ParseJson()
        .WriteJson()
        .CatchExceptions()
        .OrderBy(t => t.Text)
        .RequireAllInput(t => !string.IsNullOrWhiteSpace(t.Text))
        .Get()
        .DeleteByPrimaryKey()
        .PatchUpdateByPrimaryKey()
        .PostCreate(create => {
            create.Default(p => p.Created, () => DateTime.Now);
        });
});
// </step6>
// <step7>
// --- Header: Startup.cs ---
app.AddEFCoreSeltzr<Todo, TodoContext>("todos", api => {
    api
        .ParseJson()
        .WriteJson()
        .CatchExceptions()
        .OrderBy(t => t.Text)
        .RequireAllInput(t => !string.IsNullOrWhiteSpace(t.Text))
        .Get()
        .Get("/random", random => {
            random.Filter(d => d.Skip(new Random().Next(0, d.Count())).Take(1));
        })
        .DeleteByPrimaryKey()
        .PatchUpdateByPrimaryKey()
        .PostCreate(create => {
            create.Default(p => p.Created, () => DateTime.Now);
        });
});
// </step7>