---
uid: operations
---
# Operations
The Operation is the eighth step of the request flow, in between pre- and post-operation Actions. The operation represents the primary function of the route, whether to create, update, delete, or have some other side-effect on the data source. It is passed in the <xref:Seltzr.Context.ApiContext`2> and the dataset to operate on. When creating or updating, this dataset is often ignored in favor of the parsed models available in the <xref:Seltzr.Context.ApiContext`2>. However, in other cases, like when deleting, this dataset determines which models to operate on. Regardless of the source of the models, an Operation always returns an <xref:System.Collections.Generic.IEnumerable`1> containing the models it affected. This dataset is then passed to the post-operation Actions and Result Writer. 

The Operation request flow step is optional. If no Operation is provided, the dataset remains unaffected and the route will serialize and return the dataset. This is typically how `GET` requests are set up.

> [!IMPORTANT]
> Base Seltzr includes no Operations. Instead, use packages like `Seltzr.EntityFramework`, `Seltzr.EntityFrameworkCore`, and `Seltzr.LinqToDB` which include customized Operations for working with the ORM.

# Builder Methods
## Low-Level Methods
These methods are primarily used when you've implemented your own <xref:Seltzr.Operations.IOperation`2> and want to attach it to a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.UseOperation(Seltzr.Operations.IOperation{`0,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.UseOperation``1>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearOperation>

## High-Level Methods
There are no high-level builder methods for adding common Operations. Instead you'll typically set an Operation when creating a new route. See [Routing](xref:routing#examples) for examples.

## Examples
#### Using a custom Operation on a route
This example uses a custom Operation that updates an existing model or creates a new one.

```csharp
app.UseSeltzr<MyModel>("addorupdate", api => {
	api
		.UseOperation<CreateOrUpdate<MyModel>>()
		.CanPost();
});
```