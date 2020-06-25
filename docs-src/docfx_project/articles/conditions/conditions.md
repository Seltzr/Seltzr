---
uid: conditions
---

# Conditions
Checking conditions is the sixth step in the request flow. It occurs after filtering and before pre-operation actions. An unlimited number of Conditions may be added to a route. They are run in order against the filtered dataset and the current <xref:Seltzr.Context.IApiContext`2>. Conditions return a boolean value indicating whether or not the condition has passed, and can also throw an exception upon failure. If a condition does not pass, an exception of type <xref:Seltzr.Exceptions.ConditionFailedException> is thrown and the route stops further execution.

## Builder Methods
### Low-Level Methods
These methods are primarily used when you've implemented your own <xref:Seltzr.Conditions.ICondition`2> and want to attach it to a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddCondition(Seltzr.Conditions.ICondition{`0,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddCondition``1>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearConditions>

### High-Level Methods
**Builder Prefix:** `Require`

These are the methods you'll typically use to set conditions on a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Require(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Linq.IQueryable{`0},System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Require(System.Func{System.Linq.IQueryable{`0},System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireAll(System.Func{`0,System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireAllInput(System.Func{`0,System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireAsync(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Linq.IQueryable{`0},System.Threading.Tasks.Task{System.Boolean}},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireAsync(System.Func{System.Linq.IQueryable{`0},System.Threading.Tasks.Task{System.Boolean}},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireAtLeast(System.Int32,System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireExactly(System.Int32,System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireExactlyOne(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireExactlyOneInput(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireInput(System.Func{`0[],System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireInputAsync(System.Func{`0[],System.Threading.Tasks.Task{System.Boolean}},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireInputHasAtLeast(System.Int32,System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireInputHasExactly(System.Int32,System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireNonEmpty(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireNonEmptyInput(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireParameter``1(Seltzr.ParameterRetrievers.ParameterRetriever,System.Func{``0,System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireParameterOpt``1(Seltzr.ParameterRetrievers.ParameterRetriever,System.Func{``0,System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireQuery``1(System.String,System.Func{``0,System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireQueryOpt``1(System.String,System.Func{``0,System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireRoute``1(System.String,System.Func{``0,System.Boolean},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireRouteOpt``1(System.String,System.Func{``0,System.Boolean},System.String)>

> [!WARNING]
> Even though the `RequireProperty` methods share a similar name and purpose with the condition methods, they actually set [ParsingOptions](xref:parsing#parsingoptions) on the <xref:Seltzr.Parsers.IBodyParser`1> and do *not* enforce conditions.

## Examples
#### Requiring a condition on every element in the dataset
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireAll(System.Func{`0,System.Boolean},System.String)> method can be used to setup one Condition that checks every single element in the dataset.

The following example ensures that every element in the filtered dataset was created recently.
```csharp
app.UseSeltzr<BlogPost>(api => {
    api.RequireAll(m => m.DateCreated < DateTime.Now.AddDays(-30))
});
```

The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireAllInput(System.Func{`0,System.Boolean},System.String)> works in the same way with the parsed request body. 

The following example ensures that the `PublishDate` of every element in the parsed body is in the future.
```csharp
app.UseSeltzr<BlogPost>(api => {
    api.RequireAll(m => m.PublishDate > DateTime.Now)
});
```

> [!NOTE]
> Even if your body parser doesn't accept arrays, the <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireAllInput(System.Func{`0,System.Boolean},System.String)> method will still work fine.

#### Using Conditions with Filters
Conditions can be especially powerful when used in conjunction with parameter Filters.

The following example shows how to set up a Filter on a query parameter and restricting its bounds to certain range.

```csharp
app.UseSeltzr<MyModel>(api => {
    api
		.FilterByQueryEqual(m => m.Value)
		.RequireQuery<int>("value", v => v > 10 && v < 50);
});
```

> [!TIP]
> These methods accept a type parameter to parse the parameter before it's passed to the lambda method. To avoid parsing the parameter, use the `string` type.

> [!NOTE]
> Like the `FilterBy` methods, `Require` methods that work with request parameters have `Opt` alternatives that only apply the condition when the request parameter is present

#### Setting a custom error message on a Condition
All Condition builder methods have a `failureMessage` parameter that defaults to `null`, which will use a default error message. To customize the exception message and, therefore, the error message shown when using the <xref:Seltzr.ExceptionHandlers.SimpleExceptionHandler>, provide a value for the `failureMessage` parameter.

```csharp
app.UseSeltzr<MyModel>(api => {
    api.RequireAllInput(m => m.Value < 50, "Value must be less than 50 for all provided models");
});
```

#### Using the `Require` method for more complex logic
The `Require` and `RequireAsync` methods both take in a function, which either accepts the dataset or both the dataset and <xref:Seltzr.Context.IApiContext`2>, and returns a boolean value indicating whether or not the condition was met.

This example verifies that the `Value` property of models with `ValueType` set to `"Integer"` doesn't have a decimal component.
```csharp
app.UseSeltzr<MyModel>(api => {
    api.Require(d => d.Where(m => m.ValueType == "Integer").All(m => (m.Value - (int)m.Value) < 0.00001));
});
```