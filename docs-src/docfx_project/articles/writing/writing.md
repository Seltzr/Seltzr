---
uid: result-writers
---

# Result Writers
Writing the result is the last step of the request flow. It occurs after post-operation Actions. The Result Writer is responsible for serializing the affected models and optional [wrapped response body](xref:response-wrapping). Only one result writer may be attached to a route. A Result Writer is required for a route to be accessible. If serialization fails, an exception of type <xref:Seltzr.Exceptions.WritingFailedException> is thrown.

## FormattingOptions
Every result writer accepts a <xref:Seltzr.Options.FormattingOptions> object, which specifies the properties on the model that should be serialized and whether to [strip the array if the dataset only contains one element](#returning-a-single-element instead-of-an-array). Methods for controlling this object are listed below in [Formatting Options](#formatting-options).

## Builder Methods
### Low-Level Methods
These methods are primarily used when you've implemented your own <xref:Seltzr.Results.IResultWriter`2> and want to attach it to a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.UseResultWriter(Seltzr.Results.IResultWriter{`0,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.UseResultWriter``1>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearResultWriter>

### High-Level Methods
#### Result Writers
**Builder Prefix:** `Write`

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteJson>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteJson(System.Text.Json.JsonSerializerOptions)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteJsonOrXml(System.Text.Json.JsonSerializerOptions)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteNumberAffected(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteString(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteXml>

#### Formatting Options
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Include(System.Linq.Expressions.Expression{System.Func{`0,System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Include(System.Reflection.PropertyInfo)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.IncludeAll>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Omit(System.Linq.Expressions.Expression{System.Func{`0,System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Omit(System.Reflection.PropertyInfo)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.OmitAll>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.StripArrayIfSingleResult(System.Boolean)>

**Extension Methods:**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.IncludePrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.OmitPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>**

> [!IMPORTANT]
> Don't confuse `Omit` and `Include` with `Require`, `Ignore`, and `Optional`. The former methods work with response bodies, the latter with request bodies.

> [!NOTE]
> Methods in **bold** are only available with an ORM-backed version of Seltzr, e.g `Seltzr.EntityFrameworkCore`

## Examples
### Result Writing
#### Writing JSON or XML depending on the request
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteJsonOrXml(System.Text.Json.JsonSerializerOptions)> method will determine the result writer to use using the following criteria in order:
1. The `Accept` header was specified and `application/json` or `application/xml` is accepted.
2. The `format` query parameter is specified and set to `json` or `xml` (case insensitive).
3. The `Content-Type` header was specified and set to `application/json` or `application/xml`.
4. JSON is used as the default result writer

```csharp
app.UseSeltzr<MyModel>(api => {
    api.WriteJsonOrXml();
});
```

```rest
--- Header: Request nocopy---
POST https://localhost:5000/
Content-Type: application/xml

...
```
```xml
--- Header: Response nocopy---
<ArrayOfMyModel>
	...
</ArrayOfMyModel>
```

#### Writing the number of affected models
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteNumberAffected(System.String)> method can be used with a template string to write out the number of affected models.

```csharp
app.UseSeltzr<MyModel>(api => {
    api
		.WriteNumberAffected("{0} Model(s) Updated")
		.PostUpdateByPrimaryKey();
});
```

```rest
--- Header: Request nocopy---
POST https://localhost:5000/5

...
```
```xml
--- Header: Response nocopy---
1 Model(s) Updated
```

#### Choosing a result writer based on a query parameter
The <xref:Seltzr.Results.QueryDependentResultWriter`2> class supports using different result writers based on a query parameter.

This example defines a `fmt` parameter which can either write `"json"` or `"xml"`. Since no value is provided for the `defaultIndex` parameter, if the parameter is not specified, an exception is thrown.
```csharp
QueryDependentResultWriter<TModel, TUser> QueryDependentResultWriter = new QueryDependentResultWriter<TModel, TUser>(
	"fmt", 
	new[] {"json", "xml"}, 
	new IResultWriter<TModel, TUser>[] { new JsonResultWriter<TModel>(), new XmlResultWriter<TModel>() },
	caseSensitive: true);

app.UseSeltzr<MyModel>(api => {
    api.UseResultWriter(QueryDependentResultWriter);
});
```

```rest
--- Header: Request nocopy---
GET https://localhost:5000/?fmt=json
```
```json
--- Header: Response nocopy---
[{
	...
}]
```

```rest
--- Header: Request nocopy---
GET https://localhost:5000/?fmt=xml
```
```xml
--- Header: Response nocopy---
<ArrayOfMyModel>
	...
</ArrayOfMyModel>
```

```rest
--- Header: Request nocopy---
GET https://localhost:5000/
```
```xml
--- Header: Response nocopy---
Request aborted. Cannot serialize response to request
```

### Modifying Formatting Options
#### Omitting properties from the response body
Unless previous calls to `Include` have affected the formatting options, a call to `Omit` will include every property except for the omitted one.

```csharp
app.UseSeltzr<MyModel>(api => {
    api
		.WriteJson()
		.Omit(m => m.Value)
		.CanGet();
});
```

```rest
--- Header: Request nocopy---
GET https://localhost:5000/
```
```json
--- Header: Response nocopy---
[
	{
		"Id": 4,
		"Token": "52d10081-8730-4a04-8725-f2aefb6dfac8"
	},
	{ ... }
]
```

#### Including a single property on the response body
Unless previous calls to `Omit` have affected the formatting options, a call to `Include` will omit every property except for the included one.

```csharp
app.UseSeltzr<MyModel>(api => {
    api
		.WriteJson()
		.Include(m => m.Id)
		.Include(m => m.Value)
		.CanGet();
});
```

Now only two properties (`Id` and `Value`) will be included in the response.

```rest
--- Header: Request nocopy---
GET https://localhost:5000/
```
```json
--- Header: Response nocopy---
[
	{
		"Id": 4,
		"Value": 8
	},
	{ ... }
]
```

#### Returning a single element instead of an array
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.StripArrayIfSingleResult(System.Boolean)> method can be used to strip the array from the response if the dataset only contains a single element. So, if a route will only ever logically return a single element, it makes sense to use this method.

```csharp
app.UseSeltzr<MyModel>(api => {
    api
		.WriteJson()
		.GetByPrimaryKey(getOptions => getOptions.StripArrayIfSingleResult())
});
```

```rest
--- Header: Request nocopy---
GET https://localhost:5000/4
```
```json
--- Header: Response nocopy---
{
	"Id": 4,
	"Value": 8,
	"Token": "52d10081-8730-4a04-8725-f2aefb6dfac8"
}
```

> [!TIP]
> To throw an error when no elements are found, use the <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireExactlyOne(System.String)> method.