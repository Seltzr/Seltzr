---
uid: actions
---

# Actions
You can add Actions to a route that run before and after the [Operation](xref:operations). Actions are provided the current <xref:Seltzr.Context.IApiContext`2> and dataset and don't return anything. Pre-operation Actions are the seventh step of the request flow, after condition checks, and are typically used to modify the parsed request body before it's sent to the Operation. Post-operation Actions are the ninth step of the request flow, before result writing, and are typically used to modify the API response before it's sent.

## Builder Methods
### Low-Level Methods
These methods are primarily used when you've implemented your own <xref:Seltzr.Actions.IPostOpAction`2> or <xref:Seltzr.Actions.IPreOpAction`2> and want to attach it to a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddPostOpAction(Seltzr.Actions.IPostOpAction{`0,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddPostOpAction``1>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddPreOpAction(Seltzr.Actions.IPreOpAction{`0,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddPreOpAction``1>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearPostOpActions>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearPreOpActions>

### High-Level Methods
**Builder Prefix:** `Before`, `After`

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.After(System.Action{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AfterAsync(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Threading.Tasks.Task})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Before(System.Action{Seltzr.Context.IApiContext{`0,`1},System.Linq.IQueryable{`0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.BeforeAsync(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Linq.IQueryable{`0},System.Threading.Tasks.Task})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Deprecate(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ForEach(System.Action{`0})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ForEachAsync(System.Func{`0,System.Threading.Tasks.Task})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ForEachInput(System.Action{`0})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ForEachInputAsync(System.Func{`0,System.Threading.Tasks.Task})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetHeader(System.String,System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.String})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetHeader(System.String,System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetHeaderAsync(System.String,System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Threading.Tasks.Task{System.String}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValue(System.Linq.Expressions.Expression{System.Func{`0,System.Object}},Seltzr.ParameterRetrievers.ParameterRetriever)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValue(System.Linq.Expressions.Expression{System.Func{`0,System.Object}},System.Func{Seltzr.Context.IApiContext{`0,`1},System.Object})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValue(System.Reflection.PropertyInfo,Seltzr.ParameterRetrievers.ParameterRetriever)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValue(System.Reflection.PropertyInfo,System.Func{Seltzr.Context.IApiContext{`0,`1},System.Object})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueAsync(System.Linq.Expressions.Expression{System.Func{`0,System.Object}},System.Func{Seltzr.Context.IApiContext{`0,`1},System.Threading.Tasks.Task{System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueAsync(System.Reflection.PropertyInfo,System.Func{Seltzr.Context.IApiContext{`0,`1},System.Threading.Tasks.Task{System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueQuery(System.Linq.Expressions.Expression{System.Func{`0,System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueQuery(System.Linq.Expressions.Expression{System.Func{`0,System.Object}},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueQuery(System.Reflection.PropertyInfo)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueQuery(System.Reflection.PropertyInfo,System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueRoute(System.Linq.Expressions.Expression{System.Func{`0,System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueRoute(System.Linq.Expressions.Expression{System.Func{`0,System.Object}},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueRoute(System.Reflection.PropertyInfo)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValueRoute(System.Reflection.PropertyInfo,System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseCountValue>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValue(System.String,System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Object})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValue(System.String,System.Object)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValue``1(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Object})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValue``1(System.Object)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValueAsync(System.String,System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Threading.Tasks.Task{System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValueAsync``1(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Threading.Tasks.Task{System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteVersion(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteVersionHeader(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteVersionValue(System.String)>

## Examples
#### Setting a value on a parsed model before the operation
The `SetValue` methods can be used to set properties' values on parsed models before the operation.

This example will always set the `Value` of the model to `4`
```csharp
app.UseSeltzr<MyModel>(api => {
    api.SetValue(m => m.Value, (c) => 4);
});
```

The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetValue(System.Linq.Expressions.Expression{System.Func{`0,System.Object}},System.Func{Seltzr.Context.IApiContext{`0,`1},System.Object})> method uses a `Func` parameter to get the desired value of the property, which is passed the current API context.

This example uses a service to get the desired value for the `Value` property.
```csharp
app.UseSeltzr<MyModel>(api => {
    api.SetValue(m => m.Value, (c) => c.Services.GetRequiredService<IValueManager>().GetNextValue());
});
```

The `SetValueQuery` and `SetValueRoute` methods can also be used to set properties using request parameters.
```csharp
app.UseSeltzr<MyModel>("{val}", api => {
    api.SetValueQuery(m => m.ChildId); // query parameter name is inferred to be "childId"
    api.SetValueRoute(m => m.Value, "val");
});
```

#### Modifying values of parsed models before the operation
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ForEachInput(System.Action{`0})> and <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ForEachInputAsync(System.Func{`0,System.Threading.Tasks.Task})> methods can be used to modify existing values on parsed models before the operation.

This example trims the `Name` field of a model before it's created or updated.
```csharp
app.UseSeltzr<MyModel>(api => {
    api.ForEachInput(m => m.Name = m.Name.Trim());
});
```

#### Logging events with Actions
The `After` and `Before` methods can be used for logging events.

This example injects an <xref:Microsoft.Extensions.Logging.ILogger`1> to log after the operation.
```csharp
app.UseSeltzr<MyModel>(api => {
    api.After((c, d) => {
		c.Services.GetRequiredService<ILogger<CreateOperation>>.LogInformation("Created {count} models", d.Count());
	});
});
```

#### Setting response values
See <xref:response-wrapping> for details on using the `WriteValue` methods.