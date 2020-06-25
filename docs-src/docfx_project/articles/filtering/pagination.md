---
uid: pagination
---

# Pagination
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2> class includes a number of `Paginate` methods which can be used to easily add pagination to an API route.

> [!TIP]
> The `Paginate` methods will automatically set the response values marked with <xref:Seltzr.Responses.Attributes.CurrentPageAttribute> and <xref:Seltzr.Responses.Attributes.TotalPagesAttribute> if the response is wrapped. [Wrapping Responses](xref:response-wrapping) is a good idea with pagination so that API consumers can navigate between pages more easily. The examples on this page will all implicitly use wrapped responses.

## Usage
The following examples show typical usage of the `Paginate` methods.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate(System.String,System.Int32)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate(System.String,System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate(System.String,System.String,System.Int32)>

Without any parameters, the <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate> method will return all models with no parameters, or, if the `count` query parameter is specified, `count` number of models. Which models to show is determined by the `page` query parameter, which starts at `1`.

```csharp
app.UseSeltzr<MyModel>(api => {
	api
		.Paginate()
		.CanGet();
});
```

The following example requests show the behavior of this method:
```rest
--- Header: Request nocopy---
GET https://localhost:5001/
```

```json
--- Header: Response nocopy---
{
	"Elements": [
		...
	],
	"Count": 250
}
```

```rest
--- Header: Request nocopy---
GET https://localhost:5001/?page=2&count=50
```

```json
--- Header: Response nocopy---
{
	"Elements": [
		...
	],
	"CurrentPage": 2,
	"TotalPages": 5,
	"Count": 50
}
```

Additionally, the overload method <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate(System.String,System.String)> takes in custom parameter names.

```csharp
app.UseSeltzr<MyModel>(api => {
	api
		.Paginate("pageNumber", "max")
		.CanGet();
});
```

To set a maximum page size limit, use the <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate(System.String,System.String,System.Int32)> overload.

```csharp
app.UseSeltzr<MyModel>(api => {
	api
		.Paginate("page", "count", 50)
		.CanGet();
});
```

```rest
--- Header: Request nocopy---
GET https://localhost:5001/?count=60
```

```json
--- Header: Response nocopy---
{
	"Elements": [
		...
	],
	"CurrentPage": 1,
	"TotalPages": 5,
	"Count": 50
}
```

To prevent custom page sizes, use the <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Paginate(System.String,System.Int32)> overload.

```csharp
app.UseSeltzr<MyModel>(api => {
	api
		.Paginate("page", 50)
		.CanGet();
});
```

The count parameter is now ignored:
```rest
--- Header: Request nocopy---
GET https://localhost:5001/?count=25
```

```json
--- Header: Response nocopy---
{
	"Elements": [
		...
	],
	"CurrentPage": 1,
	"TotalPages": 5,
	"Count": 50
}
```