---
uid: routing
---
# Routing
Routing is the first step of the request flow. It is implemented using the [ASP.NET Core routing engine](xref:fundamentals/routing). Routing matches a route defined with one of the many routing builder methods to the request path of an incoming HTTP request.

- Every builder method that sets up a route can take in a string parameter that defines the route pattern. The route pattern is compatible with [ASP.NET core templates](xref:fundamentals/routing#route-template-reference) and defines which request paths will match with the route.
- For a route to actually be matched and executed by Seltzr, it **must** include the following
	- At least one supported request method (`GET`, `POST`, etc.)
	- A [Result Writer](xref:result-writers) to serialize a response

## Builder Methods
### Low-Level Methods
These methods are for more low-level routing, that don't define any request methods or operations.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FlatMap>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FlatMap(System.Action{Microsoft.AspNetCore.Builder.IEndpointConventionBuilder})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FlatMap(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FlatMap(System.String,System.Action{Microsoft.AspNetCore.Builder.IEndpointConventionBuilder})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.MapRoute(System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.MapRoute(System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}},System.Action{Microsoft.AspNetCore.Builder.IEndpointConventionBuilder})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.MapRoute(System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.MapRoute(System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}},System.Action{Microsoft.AspNetCore.Builder.IEndpointConventionBuilder})>

### Request Methods
These methods set request methods for the current route, but don't create a new route
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddRequestMethod(System.String)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CanDelete>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CanGet>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CanPatch>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CanPost>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CanPut>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearRequestMethods>

### Basic Routing
These methods setup a new route that supports the given request method. They each have three overloads:
- The first accepts no parameters, and sets up a route on the same pattern with no additional options. In practice, this is usually equivalent to calling `Can{Method}` for the method.
- The second accepts a callback for configuring the route, and sets up the route on the same pattern as its parent
- The third accepts both a route pattern and a callback, and sets up the route on a pattern that combines the parent route pattern with the given route pattern.

**GET**
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupAnonymousGet>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupAnonymousGet(System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupAnonymousGet(System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupGet>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupGet(System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupGet(System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>

**POST**
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupPost>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupPost(System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupPost(System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>

**PUT**
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupPut>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupPut(System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupPut(System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>

**PATCH**
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupPatch>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupPatch(System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupPatch(System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>

**DELETE**
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupDelete>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupDelete(System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupDelete(System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})>

### Routing with Operations
These methods are only available when using an ORM-backed version of Seltzr, and set up a request method and operation and sometimes additional configuration. They generally follow the same 3-overload pattern as the methods above.

**DELETE**
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.Delete``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.DeleteByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>

**GET**
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.Get``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.Get``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.Get``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.GetByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.GetByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.GetByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.GetByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.GetByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.GetByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.GetByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.GetByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.GetByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>

**PATCH**
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByParam``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByParam``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},Seltzr.ParameterRetrievers.ParameterRetriever,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByParam``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PatchUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>

**POST (Create)**
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostCreate``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostCreate``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>

**POST (Update)**
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByParam``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByParam``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},Seltzr.ParameterRetrievers.ParameterRetriever,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByParam``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PostUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>

**PUT**
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateBy``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByBody``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByParam``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByParam``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},Seltzr.ParameterRetrievers.ParameterRetriever,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByParam``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,Seltzr.ParameterRetrievers.ParameterRetriever[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKey``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByPrimaryKeyQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}},System.String,System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[])>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>
- <xref:Seltzr.Extensions.SeltzrOrmOptionsBuilderExtensions.PutUpdateByQuery``2(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1},System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}[],System.String[],System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,``1}})>

> [!NOTE]
> There are **a lot** of builder methods for creating routes with an ORM--don't try to memorize them all! Instead, remember the general categories that they fall into:
> * Updating
>	* UpdateBy 
> TODO!

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

## Examples
Also todo
* generic routing
* by primary key (route, query, body)
* update that doesn't use primary key
* creation
* get