---
uid: response-wrapping
---

# Response Wrapping
Though serializing bare models and arrays of models works fine for many APIs, Seltzr also supports wrapping responses in another object so that additional information can be included with the response.

## Example Usage
For example, take the following API and sample response:
```csharp
app.UseSeltzr<MyModel>(api => {
	api.CanGet();
});
```

```rest
--- Header: Request nocopy---
GET https://localhost:5000/
```
```json
--- Header: Response nocopy---
[
	{ ... },
	{ ... },
	{ ... }
]
```

This API can be wrapped with the built-in <xref:Seltzr.Responses.BasicResponse`1> class to include information like version number and result count:
```csharp
app.UseSeltzr<MyModel>(api => {
	api
		.WrapResponses()
		.WriteResponseCountValue()
		.WriteVersionValue("0.1.0")
		.CanGet();
});
```

```rest
--- Header: Request nocopy---
GET https://localhost:5000/
```
```json
--- Header: Response nocopy---
{
	"Elements": [
		{ ... },
		{ ... },
		{ ... }
	],
	"Count": 3,
	"Version": "0.1.0"
}
```

## The <xref:Seltzr.Responses.Response`1> Class
The <xref:Seltzr.Responses.Response`1> class is the base class for all wrapped responses. It includes utility methods for setting response values like `Set` and `SetString`, and a method for populating the response with the dataset. A subclass of <xref:Seltzr.Responses.Response`1> can include many properties, but only the ones that have been `Set` will be serialized by a Result Writer. Additionally, a subclass should include a property of type `TModel`, `TModel[]`, or both, where `TModel` is the type of the model class used for the API. The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WrapResponse> method will use the <xref:Seltzr.Responses.BasicResponse`1> class to wrap responses, but the <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WrapResponse``1> overload can be used to wrap responses in any custom class derived from <xref:Seltzr.Responses.Response`1>.

## Setting Response Values
Any public read-write property in a class derived from <xref:Seltzr.Responses.Response`1> can be marked as a response value by adding an attribute to it. [Several](xref:Seltzr.Responses.Attributes) are included with Seltzr, but any attribute that can be used on a property works. The <xref:Seltzr.Responses.Attributes.ResponseValueAttribute> can also be used to define custom properties without creating new attribute classes. The `TModel` properties need not be marked with attributes.

### Example
```csharp
public class MyResponse : Response<MyModel> {
	[ApiVersion]
	public string? ApiVersion { get; set; }
	
	[ResponseValue("CustomData")]
	public int? CustomData { get; set; }

	public TModel[]? Elements { get; set; }
}
```

> [!TIP]
> You can find the source code of the <xref:Seltzr.Responses.BasicResponse`1> class [here](https://github.com/Seltzr/Seltzr/blob/master/Seltzr/Responses/BasicResponse.cs).

### Setting Values with <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2>
Many builder methods are available for adding [Post-Operation Actions](xref:actions) that set response values.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Deprecate(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseCountValue>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValue(System.String,System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Object})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValue(System.String,System.Object)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValue``1(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Object})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValue``1(System.Object)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValueAsync(System.String,System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Threading.Tasks.Task{System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseValueAsync``1(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Collections.Generic.IEnumerable{`0},System.Threading.Tasks.Task{System.Object}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteVersion(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteVersionValue(System.String)>

> [!NOTE]
> The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteVersion(System.String)> method will also set the `X-Api-Version` header on responses.

Some of them, like <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Deprecate(System.String)> and <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.WriteResponseCountValue> use [built-in attributes](xref:Seltzr.Responses.Attributes), but the `WriteResponseValue` methods work with any attribute or <xref:Seltzr.Responses.Attributes.ResponseValueAttribute> value. Using the above response, we can set both `ApiVersion` and `CustomData` in the configuration method.
```csharp
app.UseSeltzr<MyModel>(api => {
	api
		.WrapResponses<MyResponse>()
		.WriteVersionValue("1.0")
		.WriteResponseValue("CustomData", 5)
		.CanGet();
});
```

### Setting Values with <xref:Seltzr.Context.IApiContext`2>
The <xref:Seltzr.Responses.Response`1> for the current request is available on the <xref:Seltzr.Context.IApiContext`2.Response> property of the <xref:Seltzr.Context.IApiContext`2>. If the request is not wrapped, this property will be `null`. Using the above example, we can set both `ApiVersion` and `CustomData` in a filter.

```csharp
app.UseSeltzr<MyModel>(api => {
	api
		.WrapResponses<MyResponse>()
		.Filter((ctx, d) => {
			ctx.Response.Set<ApiVersionAttribute>("1.0");
			ctx.Response.SetString("CustomData", "5");
			return MyFilter(d);
		})
		.CanGet();
});
```

> [!TIP]
> The <xref:Seltzr.Responses.Response`1.SetString(System.String,System.String)> and <xref:Seltzr.Responses.Response`1.SetString``1(System.String)> methods will attempt to parse the given string to the correct type before setting it. In the above example, `"5"` is parsed to the integer value `5`.