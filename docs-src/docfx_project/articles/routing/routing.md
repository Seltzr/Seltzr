---
uid: routing
---

Routing is the first step of the request flow. It is implemented using the <xref:AspNetCore.Routing> engine. Routing matches a route defined with one of the many routing builder methods to the request path of an incoming HTTP request.

- Every builder method that sets up a route can take in a string parameter that defines the route pattern. The route pattern is compatible with [ASP.NET core templates](xref:fundamentals/routing#route-template-reference) and defines which request paths will match with the route.
- For a route to actually be matched and executed by Seltzr, it **must** include the following
	- At least one supported request method (`GET`, `POST`, etc.)
	- A [Result Writer](xref:result-writers) to serialize a response

## Builder Methods
> [!IMPORTANT]
> Options set by builder methods *cascade* through routes. This means that that the innermost route in the below example inherits all of the options set **before** it, including the route pattern, set by its parent routes.
> ```csharp
> app.UseSeltzr<MyModel>("v1", api => {
>     api
> 		.MapRoute("alpha", alpha => {
> 			// this route, /v1/alpha, will NOT parse JSON or use GET.
> 		})
> 		.CanGet()
> 		.ParseJson()
> 		.MapRoute("beta", beta => {
> 			// this route, /v1/beta, WILL parse JSON and support GET.
> 		});
> });
> ```