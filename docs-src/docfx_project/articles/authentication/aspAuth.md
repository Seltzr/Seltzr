---
uid: aspAuth
---

# Authenticating with ASP.NET Core Identity
Seltzr supports authenticating with ASP.NET Core's default identity provider using the <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthAspNetIdentity>, <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthAspNetIdentity(System.String,System.String[])>, and <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AuthAspNetIdentityRole(System.String[])> methods.

## Examples
The following examples assume that `IdentityUser` is the class name of the user object used by ASP.NET Core
### Authenticating with any signed in user
Any user who is currently authenticated to the web server may access the API.
```csharp
options.UseSeltzr<MyModel, IdentityUser>(options => {
	options
		.AuthAspNetIdentity()
		.SetupGet("/get", authed => authed.WriteString("Authenticated Content"));
})
```

### Authenticating users by role
This example will only allow users with the role `api` to access the API. Note that the name of this method is AuthAspNetIdentity**Role**. The method takes a `params` argument, so multiple roles may be specified.
```csharp
options.UseSeltzr<MyModel, IdentityUser>(options => {
	options
		.AuthAspNetIdentityRole("api")
		.SetupGet("/get", authed => authed.WriteString("Authenticated Content"));
})
```

### Authenticating users by policy and role
This example will only allow users who fulfill the policy `policy` and the roles `role1` and `role2` to access the API. Roles may be omitted to just check policy.
```csharp
options.UseSeltzr<MyModel, IdentityUser>(options => {
	options
		.AuthAspNetIdentity("policy", "role1", "role2")
		.SetupGet("/get", authed => authed.WriteString("Authenticated Content"));
});
```

### Redirecting to the login page if authentication fails
By default, if authentication fails, the <xref:Seltzr.ExceptionHandlers.SimpleExceptionHandler> will return a `401 Unauthorized` response. However, this example will redirect users to a sign-in page if authentication fails.
```csharp
options.UseSeltzr<MyModel, IdentityUser>(options => {
	options
		.AuthAspNetIdentity()
		.Catch<AuthFailedException>((e, c) => {
			c.Response.Redirect("/auth/login");
			return false; // halt further request execution
		})
		.CatchExceptions();
})
```

### Restricting routes by role
This example allows authenticated users to access all routes except for those on the `/admin` pattern, which requires a role of `"admin"` to access.
```csharp
options.UseSeltzr<MyModel, IdentityUser>(options => {
	options
		.AuthAspNetIdentity() // permit all authenticated users
		.SetupGet("/get", authed => authed.WriteString("Authenticated Content"))
		.MapRoute("/admin", admin => {
			admin
				.ClearAuthProviders()
				.AuthAspNetIdentityRole("admin") // just administrators may access /admin
				.SetupGet(authed => authed.WriteString("Admin Content"));
		});
});
```

> [!IMPORTANT]
> The `/admin` route must call `ClearAuthProviders()` or the call to `MapRoute` must occur before the call to the general `AuthAspNetIdentity()`. Otherwise, because options cascade, the `/admin` route would have both the general Auth Provider and role specific Auth Provider, and any authenticated user could access the `/admin` routes. For example:
> ```csharp
> // DON'T do this!
> options.UseSeltzr<MyModel, IdentityUser>(options => {
> 	options
> 		.AuthAspNetIdentity() 
> 		.MapRoute("/admin", admin => {
> 			admin
> 				.AuthAspNetIdentityRole("admin")
> 				.SetupGet(authed => authed.WriteString("Uh-oh! Any user can access this!"));
> 		});
> });
> ```
