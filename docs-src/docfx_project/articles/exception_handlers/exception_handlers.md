---
uid: exception-handlers
---

# Exception Handlers
An unlimited number of Exception Handlers can be added to a route to catch exceptions thrown during request execution. An exception handler can return three values indicating the status of the request:

- A return value of `true` indicates that the exception should **not halt** request execution, and that the request should continue with the next route registered for the request path. 
	- If `true` is returned when there are no routes left, the server will return an empty response.
- A return value of `false` indicates that request execution should halt immediately and not call any other exception handlers.
	- In this case, the exception handler **should** write directly to the response body with a message indicating why the request failed. Otherwise no content will be returned.
- A return value of `null` indicates that the exception was not handled here and the next exception handler registered for the route should be called.
	 - If `null` is returned for the last registered exception handler, it has the same effect as returning `true`.

## Builder Methods
### Low-Level Methods
These methods are primarily used when you've implemented your own <xref:Seltzr.ExceptionHandlers.IExceptionHandler> and want to attach it to a route.

- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddExceptionHandler(Seltzr.ExceptionHandlers.IExceptionHandler)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.AddExceptionHandler``1>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.ClearExceptionHandlers>
### High-Level Methods
**Builder Prefix:** `Catch`
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch(System.Action{System.Exception,Microsoft.AspNetCore.Http.HttpContext})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch(System.Action{System.Exception})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch(System.Func{System.Exception,Microsoft.AspNetCore.Http.HttpContext,System.Nullable{System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch(System.Func{System.Exception,System.Nullable{System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch``1(Seltzr.ExceptionHandlers.IExceptionHandler)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch``1(System.Action{System.Exception,Microsoft.AspNetCore.Http.HttpContext})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch``1(System.Action{System.Exception})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch``1(System.Func{System.Exception,Microsoft.AspNetCore.Http.HttpContext,System.Nullable{System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch``1(System.Func{System.Exception,System.Nullable{System.Boolean}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync(System.Func{System.Exception,Microsoft.AspNetCore.Http.HttpContext,System.Boolean,System.Threading.Tasks.Task{System.Nullable{System.Boolean}}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync(System.Func{System.Exception,Microsoft.AspNetCore.Http.HttpContext,System.Threading.Tasks.Task{System.Nullable{System.Boolean}}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync(System.Func{System.Exception,Microsoft.AspNetCore.Http.HttpContext,System.Threading.Tasks.Task})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync(System.Func{System.Exception,System.Threading.Tasks.Task{System.Nullable{System.Boolean}}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync(System.Func{System.Exception,System.Threading.Tasks.Task})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync``1(System.Func{System.Exception,Microsoft.AspNetCore.Http.HttpContext,System.Boolean,System.Threading.Tasks.Task{System.Nullable{System.Boolean}}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync``1(System.Func{System.Exception,Microsoft.AspNetCore.Http.HttpContext,System.Threading.Tasks.Task{System.Nullable{System.Boolean}}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync``1(System.Func{System.Exception,Microsoft.AspNetCore.Http.HttpContext,System.Threading.Tasks.Task})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync``1(System.Func{System.Exception,System.Threading.Tasks.Task{System.Nullable{System.Boolean}}})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync``1(System.Func{System.Exception,System.Threading.Tasks.Task})>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchExceptions(System.Boolean)>
- <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.FailOnInvalidAuth>

## Examples
#### Using a default exception handler
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchExceptions(System.Boolean)> method adds the <xref:Seltzr.ExceptionHandlers.SimpleExceptionHandler> to the route, which catches all exceptions, writing a message for most exceptions, but continuing request execution if authentication fails and there are more routes registered for the path. See <xref:authentication> for more details.

```csharp
app.UseSeltzr<MyModel>(api => {
    api
		.CatchExceptions()
		.FilterByQueryEqual(m => m.Id);
});
```

```rest
--- Header: Request nocopy---
GET https://localhost:5000/?id=20000000000
```
```xml
--- Header: Response nocopy---
Unable to parse parameter value "20000000000"
Reason: Value was either too large or too small for an Int32.
```

#### Catching all exceptions
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync(System.Func{System.Exception,System.Threading.Tasks.Task{System.Nullable{System.Boolean}}})> method will add an exception handler that is called for all exceptions

```csharp
app.UseSeltzr<MyModel>(api => {
    api
		.CatchAsync(async (e, c) => {
			await c.Response.WriteAsync("An error occurred");
			return false;
		})
});
```

#### Catching a specific exception
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.CatchAsync``1(System.Func{System.Exception,System.Threading.Tasks.Task{System.Nullable{System.Boolean}}})> method will add an exception handler that is called for all exceptions

```csharp
app.UseSeltzr<MyModel>(api => {
    api
		.CatchAsync<ParsingFailedException>(async (e, c) => {
			await c.Response.WriteAsync("Failed to parse request body");
			return false;
		})
});
```

#### Logging an exception, but taking no action
The <xref:Seltzr.Options.Builder.SeltzrOptionsBuilder`2.Catch(System.Action{System.Exception,Microsoft.AspNetCore.Http.HttpContext})> takes in an exception handler that doesn't return anything.
```csharp
app.UseSeltzr<MyModel>(api => {
    api
		.Catch((e, c) => {
			c.RequestServices.GetRequiredService<ILogger<MyModel>>().LogError(e, "An error occurred");
		});
});
```
