---
uid: filter-by
---

# `FilterBy` Builder Methods
Seltzr includes several `FilterBy` methods to simplify the process of comparing request parameters to properties on the model. Every method is tagged with either `Query`, meaning that a query parameter value will be used for the comparison, or `Route`, meaning that a route value will be used for the comparison.

> [!NOTE]
> There is also a more generic `FilterByParameter` method which takes in any <xref:Seltzr.ParameterRetrievers.ParameterRetriever> so that this functionality can be extended to other parameters, like headers.

## Usage
The following examples show typical usage of the `FilterBy` methods.

This example demonstrates how to create a `GET` request that returns only the blog posts belonging to a particular blog.
```csharp
app.UseSeltzr<BlogPost>("/blogs/{blogId}/posts", posts => {
	posts
		.FilterByRouteEqual(p => p.BlogId)
		.CanGet();
});
```

The `FilterBy` methods, as shown above, can infer the name of the route value to compare to the models' property by camelCasing the property name. Here, `blogId` is the inferred name of the route value for the `BlogId` property. However, the name can also be specified explicitly:

```csharp
app.UseSeltzr<BlogPost>("/blogs/{blogId}/posts", posts => {
	posts
		.FilterByQueryEqual(p => p.AuthorName, "author")
		.CanGet();
});
```

For more complex logic, use the `FilterByQuery` and `FilterByRoute` methods without **Equal**. These methods take in an additional lambda parameter that takes in the property value and the parsed parameter value and returns a boolean. This example will only return blog posts that were created after the specified `after` date.
```csharp
app.UseSeltzr<BlogPost>("/blog/posts", posts => {
	posts
		.FilterByQuery(p => p.DateCreated, "after", (prop, param) => prop >= param)
		.CanGet();
});
```

The above examples will throw an error if the parameter is not present. Most `FilterBy` methods can have `Opt` appended to the end of the method name. These methods will not apply the filter if the request parameter is null or not present.
```csharp
app.UseSeltzr<BlogPost>("/blog/posts", posts => {
	posts
		.FilterByQueryEqualOpt(p => p.Id)
		.CanGet();
});
```

> [!NOTE]
> To set additional restrictions on the values of request parameters, see <xref:conditions#using-conditions-with-filters>

> [!WARNING]
> `FilterBy` methods can only compare properties of the following types: `int`, `long`, `float`, `double`, `Guid`, `decimal`, `DateTime`, `bool`, and `string`. If a property with another type is used, a runtime exception will be thrown when attempting to filter.