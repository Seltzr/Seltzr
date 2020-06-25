---
uid: model-providing
---
# Model Providing
Model Providing is the fourth step in the request flow. It occurs after authentication and before filtering. A Model Provider is used to get an <xref:System.Linq.IQueryable> containing all of the models of the data source. Exactly one model provider must be attached to a route. If no model providers are provided, an exception of type <xref:Seltzr.Exceptions.ApiException> is thrown for every request on that route.

> [!NOTE]
> Even though the <xref:System.Linq.IQueryable> objects returned by Model Providers can be used to enumerate every object in the data source, Seltzr's default Model Providers return <xref:System.Linq.IQueryable> objects that are only evaluated when `ToArray` or an equivalent method is called. This means that routes that have many filters or don't use the provided models wont incur a performance penalty.

## Builder Methods
### Low-Level Methods
These methods are primarily used when you've implemented your own <xref:Seltzr.Models.IModelProvider`2> and want to attach it to a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.UseModelProvider(Seltzr.Models.IModelProvider{`0,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.UseModelProvider``1>

### High-Level Methods
There are no high-level builder methods for adding common Model Providers. Instead you'll typically set a Model Provider when first creating the API. See [Examples](#examples) for more information.

## Examples
#### Using the Entity Framework Model Provider
Both the `Seltzr.EntityFramework` and `Seltzr.EntityFrameworkCore` packages include a Model Provider that gets the <xref:Microsoft.EntityFrameworkCore.DbSet`1> for the API's model. Their extension methods on <xref:Microsoft.AspNetCore.Builder.IApplicationBuilder> will use this Model Provider automatically.

```csharp
// --- Header: Seltzr.EntityFramework ---
// Get all of the `MyModel` objects from `MyDbContext`
app.AddEntityFrameworkSeltzr<MyModel, MyDbContext>(api => {
	api.Get();
});
```

```csharp
// --- Header: Seltzr.EntityFrameworkCore ---
// Get all of the `MyModel` objects from `MyDbContext`
app.AddEFCoreSeltzr<MyModel, MyDbContext>(api => {
	api.Get();
});
```

#### Attaching a custom provider to a route
The following example sets up an API that responds to a `GET` request at the `/random` endpoint with a list of random numbers.

```csharp
app.UseSeltzr<int>("random", api => {
	api
		.UseModelProvider<RandomNumbersProvider>()
		.CanGet();
});
```