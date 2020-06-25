---
uid: filtering
---

# Filtering
Filtering is the fifth step in the request flow. It occurs after model providing and before condition checks. An unlimited number of Filters may be added to a route. They will be run in the order they were added. Each Filter takes in the dataset and <xref:Seltzr.Context.IApiContext`2> and returns a modified dataset, either directly filtered or otherwise transformed. This new dataset will be used in place of the existing one until provided to the [Operation](xref:operations). If there is no operation, the filtered dataset is returned by the API.

> [!IMPORTANT]
> When using Seltzr with an ORM like Entity Framework, avoid [client-side evaluation](https://docs.microsoft.com/en-us/ef/core/querying/client-eval#unsupported-client-evaluation) in Filters. Client-side evaluation often results in pulling more information from the database than is necessary for the request, and can result in poor performance. If you must use client-side evaluation, see [below](#explicit-client-side-evaluation) for an example that explicitly enables it.

## Builder Methods
### Low-Level Methods
These methods are primarily used when you've implemented your own <xref:Seltzr.Filters.IFilter`2> and want to attach it to a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddFilter(Seltzr.Filters.IFilter{`0,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddFilter``1>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearFilters>

### High-Level Methods
**Builder Prefix:** `Filter`

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Filter(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Linq.IQueryable{`0},System.Linq.IQueryable{`0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Filter(System.Func{System.Linq.IQueryable{`0},System.Linq.IQueryable{`0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterAsync(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Linq.IQueryable{`0},System.Threading.Tasks.Task{System.Linq.IQueryable{`0}}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterAsync(System.Func{System.Linq.IQueryable{`0},System.Threading.Tasks.Task{System.Linq.IQueryable{`0}}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByParameter``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},Seltzr.ParameterRetrievers.ParameterRetriever,System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByParameterEqual``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},Seltzr.ParameterRetrievers.ParameterRetriever)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByParameterEqualOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},Seltzr.ParameterRetrievers.ParameterRetriever)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByParameterOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},Seltzr.ParameterRetrievers.ParameterRetriever,System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByQuery``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByQuery``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.String,System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByQueryEqual``1(System.Linq.Expressions.Expression{System.Func{`0,``0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByQueryEqual``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByQueryEqualOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByQueryEqualOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByQueryOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByQueryOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.String,System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByRoute``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByRoute``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.String,System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByRouteEqual``1(System.Linq.Expressions.Expression{System.Func{`0,``0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByRouteEqual``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByRouteEqualOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByRouteEqualOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByRouteOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterByRouteOpt``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.String,System.Linq.Expressions.Expression{System.Func{``0,``0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterWhere(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FilterWhere(System.Linq.Expressions.Expression{System.Func{Seltzr.Context.IApiContext{`0,`1},`0,System.Boolean}})>
- *<xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.First>*
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Limit(System.Int32)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.LimitOne>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.LimitQuery(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.LimitQuery(System.String,System.Int32)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.OrderBy``1(System.Linq.Expressions.Expression{System.Func{`0,``0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.OrderBy``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.Collections.Generic.IComparer{``0})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.OrderByDescending``1(System.Linq.Expressions.Expression{System.Func{`0,``0}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.OrderByDescending``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.Collections.Generic.IComparer{``0})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate(System.String,System.Int32)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate(System.String,System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate(System.String,System.String,System.Int32)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Skip(System.Int32)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SkipQuery(System.String,System.Int32)>

**Extension Methods:**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.FilterByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},Seltzr.ParameterRetrievers.ParameterRetriever[])>**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.FilterByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.FilterByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String[])>**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.FilterByPrimaryKeyRoute``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.FilterByPrimaryKeyRoute``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String[])>**

> [!NOTE]
> Methods in *italics* also enforce <xref:conditions> alongside the Filter they add

> [!NOTE]
> Methods in **bold** are only available with an ORM-backed version of Seltzr, e.g `Seltzr.EntityFrameworkCore`

## Examples
#### Filtering using Route Values and Query Parameters
See <xref:filter-by> for examples on filtering using request parameters.

#### Explicit Client-Side Evaluation
Although client-side evaluation is generally discouraged, it is sometimes necessary for complex logic which can't be translated to a database query. To explicitly enable client-side evaluation, call <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FetchModels> before filtering.

```csharp
app.UseSeltzr<BlogPost>("/blogs/{blogId}/posts/recent", posts => {
	posts
		.Filter(p => p.DateCreated > DateTime.Now.AddDays(-30))
		.FetchModels()
		.FilterByQueryOpt(p => p.Author, "token", (a, t) => GetToken(a) == t)
		.CanGet();
});
```

> [!TIP]
> Call <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FetchModels> as late as possible when filtering to keep most of the evaluation on the server.

#### Using LINQ analog methods to filter the dataset
Often you'll need to filter the dataset using queries traditionally available with [LINQ](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/) methods. Seltzr includes several analogous methods for filtering, including `FilterWhere`, `Skip`, `Limit`, and `OrderBy`.

The following example uses some of those methods to return a sorted list of the first 10 users whose name starts with the letter "B": 
```csharp
app.UseSeltzr<User>("/users", posts => {
	posts
		.FilterWhere(u => u.Name.StartsWith("B"))
		.OrderBy(u => u.Name)
		.Limit(10)
		.CanGet();
});
```

> [!NOTE]
> Since filter methods are run in the order they were added, calling `OrderBy` after `Limit` would alphabetically sort the first 10 users returned by the database, not the 10 alphabetically-first users.

The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2> class also has a `LimitQuery` method which can be used to limit the number of returned models to a number specified by a query parameter.
```csharp
app.UseSeltzr<User>("/users", posts => {
	posts
		.LimitQuery("count", 50) // max count is 50
		.CanGet();
});
```

#### Using the `Filter` method for complex logic
If the other builder methods don't suit your needs, the `Filter` and `FilterAsync` methods can be used for complex filtering logic. These methods take lambdas which can be passed either just the dataset, or both the <xref:Seltzr.Context.IApiContext`2> and the dataset. The following example uses the context to get a service which filters the dataset.
```csharp
app.UseSeltzr<User>("/users", posts => {
	posts
		.Filter((ctx, data) => {
			IFilteringService Service = ctx.Services.GetRequiredService<IFilteringService>();
			return Service.Filter(data);
		})
		.CanGet();
});
```

#### Pagination
For examples on how to use the `Paginate` methods, see <xref:pagination>