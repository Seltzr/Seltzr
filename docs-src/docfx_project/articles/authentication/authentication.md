---
uid: authentication
---
# Authentication
Authentication is the third step in the request flow. It occurs after parsing and before model retrieval. Multiple Authentication Providers can be attached to a single route and will be executed in order until authentication succeeds. If no Auth Provider succeeds, an exception of type <xref:Seltzr.Exceptions.AuthFailedException> will be thrown. Authentication Providers are optional and if none are specified this step is skipped, and the <xref:Seltzr.Context.IApiContext`2.User> property of the request's <xref:Seltzr.Context.IApiContext`2> will be `null`.

> [!NOTE]
> This execution method means that if **any** Auth Provider succeeds, the request will be considered authenticated.

> [!NOTE]
> The <xref:Seltzr.ExceptionHandlers.SimpleExceptionHandler> exception handler added by the <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchExceptions(System.Boolean)> method will fall through to the next route specified for that route pattern if authentication fails. Additionally, routes with the same pattern are executed in order of the number of Auth Providers they have. This way, an anonymous user can be presented with an alternate API route for the same route pattern. See [below](#hosting-anonymous-and-authenticated-content-on-the-same-route) for an example.

## Api Context
**See Also:** <xref:Seltzr.Context.IApiContext`2> 

This step will set the <xref:Seltzr.Context.IApiContext`2.User> property on the request's <xref:Seltzr.Context.IApiContext`2> with an object containing additional user information. This can be set to any instance of the `TUser` type parameter for the API. However, Auth Providers may also return `null` even if authentication succeeds, indicating that there is no additional user information.

## [NoUser](xref:Seltzr.Auth.NoUser)

If no `TUser` type parameter is provided to the `app.UseSeltzr` method when creating the API, the user context type is set to <xref:Seltzr.Auth.NoUser>. This is a token class with no properties, but makes additional extension methods available for authentication methods that don't return a user context. See [below](#authenticating-with-nouser) for an example.

## Builder Methods
### Low-Level Methods
These methods are primarily used when you've implemented your own <xref:Seltzr.Auth.IAuthProvider`2> and want to attach it to a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddAuthProvider(Seltzr.Auth.IAuthProvider{`0,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddAuthProvider``1>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearAuthProviders>

### High-Level Methods
**Builder Prefix:** `Auth`
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Auth(System.Func{Seltzr.Context.IApiContext{`0,`1},`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthAspNetIdentity>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthAspNetIdentity(System.String,System.String[])>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthAspNetIdentityRole(System.String[])>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthAsync(System.Func{Seltzr.Context.IApiContext{`0,`1},System.Threading.Tasks.Task{`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthBasic(System.Func{System.String,System.String,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthBasicAsync(System.Func{System.String,System.String,System.Threading.Tasks.Task{`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthHeader(System.String,System.Func{System.String,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthHeaderAsync(System.String,System.Func{System.String,System.Threading.Tasks.Task{`1}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthQuery(System.String,System.Func{System.String,`1})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthQueryAsync(System.String,System.Func{System.String,System.Threading.Tasks.Task{`1}})>

**Extension Methods:**
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.Auth``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.Func{Seltzr.Context.IApiContext{``0,Seltzr.Auth.NoUser},System.Boolean})>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthAsync``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.Func{Seltzr.Context.IApiContext{``0,Seltzr.Auth.NoUser},System.Threading.Tasks.Task{System.Boolean}})>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthBasic``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.Func{System.String,System.String,System.Boolean})>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthBasic``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.String,System.String)>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthBasicAsync``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.Func{System.String,System.String,System.Threading.Tasks.Task{System.Boolean}})>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthHeader``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.String,System.Func{System.String,System.Boolean})>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthHeader``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.String,System.String)>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthHeaderAsync``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.String,System.Func{System.String,System.Threading.Tasks.Task{System.Boolean}})>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthQuery``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.String,System.Func{System.String,System.Boolean})>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthQuery``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.String,System.String)>
- <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthQueryAsync``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.String,System.Func{System.String,System.Threading.Tasks.Task{System.Boolean}})>

## Examples
#### Using ASP.NET Core Identity Authentication
See <xref:aspAuth>.

#### Hosting Anonymous and Authenticated Content on the Same Route
There are two ways of hosting both unauthenticated and authenticated routes with the same pattern.

* The first uses <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.SetupAnonymousGet(System.Action{Seltzr.Options.Builder.SeltzrOptionsBuilder{`0,`1}})> to clear all Auth Providers before setting up the route:
```csharp
options.UseSeltzr<MyModel, MyUser>(options => {
	options
		.AuthAspNetIdentity()
		.SetupGet("/get", authed => authed.WriteString("Authenticated Content"))
		.SetupAnoymousGet("/get", anon => anon.WriteString("Anonymous Content"));
});
```

* The second defines the anonymous route before adding the Auth Provider:
```csharp
options.UseSeltzr<MyModel, MyUser>(options => {
	options
		.SetupGet("/get", anon => anon.WriteString("Anonymous Content"))
		.AuthAspNetIdentity()
		.SetupGet("/get", authed => authed.WriteString("Authenticated Content"));
});
```

> [!NOTE]
> There are no `SetupAnonymous` methods for the `POST`, `PUT`, `PATCH`, and `DELETE` methods, so you must use the second option, or call <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearAuthProviders> when setting up the route.

#### Authenticating with `NoUser`
The following example uses an extension method, <xref:Seltzr.Extensions.SeltzrOptionsBuilderExtensions.AuthBasic``1(Seltzr.Options.Builder.SeltzrOptionsBuilder{``0,Seltzr.Auth.NoUser},System.String,System.String)>, which is only available when the API's user context is set to [NoUser](#nouser)

```csharp
options.UseSeltzr<MyModel>(options => {
	options
		.AuthBasic("Username", "Password1")
		.SetupGet(authed => authed.WriteString("Authenticated Content"));
});
```