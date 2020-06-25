---
uid: parsing
---
# Parsing
Body parsing is the second step in the request flow. It occurs after routing and before authentication. Multiple parsers may be attached to a single route, they will be executed in the order they were added. If one parser fails, the next parser will be used. If all parsers fail, an exception of type <xref:Seltzr.Exceptions.ParsingFailedException> will be thrown. Body parsers are optional and if none are specified this step is skipped, and the <xref:Seltzr.Context.IApiContext`2.Parsed> property of the request's <xref:Seltzr.Context.IApiContext`2> will be `null`.

## Api Context
**See Also:** <xref:Seltzr.Context.IApiContext`2> 

This step will set the <xref:Seltzr.Context.IApiContext`2.Parsed> property on the request's <xref:Seltzr.Context.IApiContext`2> to an array of <xref:Seltzr.Parsers.ParseResult`1> objects. Even if the parser doesn't accept arrays, the property will still be set to an array containing a single element. Every parse result object contains the parsed model and a list of the properties actually present in the request body.

## ParsingOptions
Every body parser accepts a <xref:Seltzr.Options.ParserOptions> object, which specifies the properties that should be ignored when parsing, defaults for properties, required properties, and whether the parser should accept arrays. Methods for controlling this object are listed below in [Parsing Options](#parsing-options).

## Builder Methods
### Low-Level Methods
These methods are primarily used when you've implemented your own <xref:Seltzr.Parsers.IBodyParser`1> and want to attach it to a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddBodyParser(Seltzr.Parsers.IBodyParser{`0})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddBodyParser``1>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearBodyParsers>
### High-Level Methods
These are the methods that you will most commonly use with your routes.
#### Parsing
**Builder Prefix:** `Parse`
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ParseJson>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ParseJson(System.Text.Json.JsonSerializerOptions)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ParseJsonArrays>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ParseJsonArrays(System.Text.Json.JsonSerializerOptions)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ParseXml>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ParseXmlAndJson(System.Boolean)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ParseXmlAndJsonArrays>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ParseXmlArrays>
#### Parsing Options
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AcceptArrays(System.Boolean)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Default``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},``0)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Default``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.Func{``0})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Default``1(System.Reflection.PropertyInfo,System.Func{``0})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Ignore(System.Linq.Expressions.Expression{System.Func{`0,System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Ignore(System.Reflection.PropertyInfo)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.IgnoreAll>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.OptionalAllProperties>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.OptionalProperty(System.Linq.Expressions.Expression{System.Func{`0,System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.OptionalProperty(System.Reflection.PropertyInfo)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireAllProperties>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireProperty(System.Linq.Expressions.Expression{System.Func{`0,System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.RequireProperty(System.Reflection.PropertyInfo)>

**Extension Methods:**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.IgnorePrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.RequirePrimaryKeyProperties``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>**
- **<xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.OptionalPrimaryKeyProperties``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>**

> [!IMPORTANT]
> Don't confuse `Require`, `Ignore`, and `Optional` with `Omit` and `Include`. The former methods work with request bodies, the latter with response bodies.

> [!NOTE]
> Methods in **bold** are only available with an ORM-backed version of Seltzr, e.g `Seltzr.EntityFrameworkCore`

## Examples
### Parsing
#### Accept JSON request bodies containing a single element
```csharp
app.UseSeltzr<MyModel>(api => {
    api.ParseJson();
});
```

Subsequent API routes will accept requests in the format:
```json
{
	"Property1": ...,
	"Property2": ...
}
```

#### Accept JSON request bodies containing a single element
```csharp
app.UseSeltzr<MyModel>(api => {
    api.ParseJsonArrays();
});
```

> [!NOTE]
> The preceding code is equivalent to:
> ```csharp
> app.UseSeltzr<MyModel>(api => {
>    api.ParseJson().AcceptArrays();
>});
> ```

Subsequent API routes will accept requests as a single object
```json
{
	"Property1": ...,
	"Property2": ...
}
```
or as an array of objects
```json
[
	{
		"Property1": ...,
		"Property2": ...
	},
	{
		"Property1": ...,
		"Property2": ...
	}
]
```

#### Accept JSON or XML request bodies
```csharp
app.UseSeltzr<MyModel>(api => {
    api.ParseXmlAndJson();
});
```

> [!NOTE]
> The preceding code is equivalent to:
> ```csharp
> app.UseSeltzr<MyModel>(api => {
>    api.ParseJson().ParseXml();
>});
> ```

> [!NOTE]
> For performance reasons, all of the parsers built-in to Seltzr require the appropriate `Content-Type` header to be set on the request. If you would like to change this behavior, you must inherit from a body parser and override the `CanParse` method.

### Modifying Parsing Options
#### Set a default value for a property
```csharp
app.UseSeltzr<MyModel>(api => {
	api.Default(model => model.Token, () => Guid.NewGuid);
});
```

> [!NOTE]
> The `Default` method has an overload that accepts an `object` instead of a lambda. Use that overload if you want the default value to be the same for every model.

An HTTP request that provides the following models:
```json
[
	{ "Value": "Value1" },
	{ "Value": "Value2" },
	{ "Value": "Value3", "Token": "608..." },
]
```

Will be parsed into three models, with the default value for `Token` having been specified for models that didn't provide one:
```json
[
	{ "Value": "Value1", "Token": "ad4..." },
	{ "Value": "Value2", "Token": "0db..." },
	{ "Value": "Value3", "Token": "608..." },
]
```

#### Setting a property regardless of its presence on the parsed model
```csharp
app.UseSeltzr<MyModel>(api => {
	api
		.Default(model => model.Token, () => Guid.NewGuid)
		.Ignore(model => model.Token);
});
```

Regardless of whether the `Token` property is present in a request body, the property will always be set to a new generated `Guid`.

An HTTP request that provides the following models:
```json
[
	{ "Value": "Value1" },
	{ "Value": "Value2", "Token": "608..." },
]
```

Will be parsed into two models with new `Guid` Tokens:
```json
[
	{ "Value": "Value1", "Token": "ad4..." },
	{ "Value": "Value2", "Token": "0db..." }
]
```
