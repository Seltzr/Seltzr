---
uid: request-flow
---
# Request Flow
Every time a request comes in to your API, it goes through a series of steps before the response content is returned to the API consumer. A quick overview of each step is provided below, but there are also more in-depth articles on each topic.

## Before You Start
This article assumes you have a basic understanding of the following topics: 

* HTTP requests, responses, and the REST architecture (See [Wikipedia](https://en.wikipedia.org/wiki/Representational_state_transfer#Applied_to_Web_services) for a quick overview)
* How to setup a basic Seltzr app (See <xref:getting-started>)

## Overview
On every request, the following 10 steps are run once the request is matched with a Seltzr route:

1. Routing
2. Parsing
3. Authentication
4. Providing Models
5. Filtering
6. Checking Conditions
7. Running Pre-Operation Actions
8. Running the Operation
9. Running Post-Operation Actions
10. Writing the Result. 

Additionally, if an error occurs at any point in this process, any registered Exception Handlers are called.

> [!IMPORTANT]
> These steps are always run in the order listed below, regardless of the order in which you add options to the API.

## Routing
**See Also:** [Routing](xref:routing)

The first step of the flow is request routing. Routing matches an API route to the path of the HTTP request. If multiple routes match the request path, then the routes are first sorted so that routes with authentication are executed before anonymous routes (See: <xref:authentication>).

### Example
The following example sets up two routes: `/v1/alpha` and `/v1/beta`.
```csharp
app.UseSeltzr<MyModel>("v1", api => {
    api.SetupGet("alpha", alpha => {
        alpha.WriteString("Hello World!");
    });
    api.SetupGet("beta", beta => {
        beta.WriteString("Hello again, World!");
    });
});
```

> [!WARNING]
> Only routes that have both a request method (`GET`, `POST`, etc.) and a [Result Writer](xref:result-writers) will be matched. In the above example, any requests to `/v1` alone will return a `404 Not Found` response.

## Parsing
**See Also:** [Parsing](xref:parsing)

Once the request has been matched with a route, the request body is parsed. If the route has any <xref:Seltzr.Parsers.IBodyParser`1> registered, then there *must* be a body to parse. If there is no request body, then a <xref:Seltzr.Exceptions.ParsingFailedException> will be thrown. If there are no body parsers registered, then this step is skipped and any request body will be ignored.

### Example
The following example sets up the API to parse JSON request bodies either as single objects, `{...}`, or arrays, `[{...}, {...}]`. Using `ParseJson` instead would only accept objects. Regardless of which method is used, the <xref:Seltzr.Context.ApiContext`2> will always contain an array of <xref:Seltzr.Parsers.ParseResult`1> objects.
```csharp
app.UseSeltzr<MyModel>("v1", api => {
    api.ParseJsonArrays();
});
```

## Authentication
**See Also:** [Authentication](xref:authentication)

After the request body is parsed, the request is authenticated if the route has one or more [Auth Providers](xref:Seltzr.Auth.IAuthProvider`2). Upon successful authentication, some auth providers return a `TUser` user object which will be stored in the <xref:Seltzr.Context.ApiContext`2> for the duration of the request.

### Example
The following example sets up an auth provider that calls a function to determine if a provided auth key is valid.
```csharp
app.UseSeltzr<MyModel>("v1", api => {
    api.AuthHeader("X-Api-Key", this.DetermineAuth)
       .AuthQuery("key", this.DetermineAuth);
});

...

private bool DetermineAuth(string key) {
    ...
}
```

> [!NOTE]
> Versions of the `Auth` methods that accept asynchronous callbacks are also available: just append `Async` to the end of the method name.

## Providing Models
**See Also:** [Model Providing](xref:model-providing)

Seltzr is, at its core, a way to connect database models to API consumers. To do this, each API that you build must have a way to retrieve a set of models. After the request is authenticated, this set is retrieved for use by the next phases of the request.

### Example
The easiest way to integrate a [Model Provider](xref:Seltzr.Models.IModelProvider`2) into your API is by using a library like [`Seltzr.EntityFramework`]({todo-nuget}).
```csharp
app.UseEntityFrameworkSeltzr<MyModel, MyDbContext>(api => {
    // Done! The entity framework model provider is already set up.
});
```

> [!NOTE]
> Even if you're not directly consuming these models, such as when creating a new object or just returning a string, the route must still have a model provider to succeed. (TODO: why? should it be like that? use empty/null array instead?)

## Filtering
**See Also:** [Filtering](xref:filtering)

Once the model set has been retrieved, any provided [Filters](xref:Seltzr.Filters.IFilter`2) are run over the dataset to ensure that only the requested models are returned or transformed.

### Example
This example sets up a request that returns the alphabetically first model.
```csharp
api.Get("/first", first => {
    first.OrderByDescending(m => m.Name).LimitOne();
});
```

> [!WARNING]
> Filters are run in the order you add them to the route. Calling `LimitOne` before `OrderBy` would not sort the elements.

## Conditions
**See Also:** [Conditions](xref:conditions)

Conditions ensure that the dataset meets a certain requirement, throwing otherwise.

### Example
This example ensures that all of the requested models are marked as available before returning them.

```csharp
api.Get(route => {
    route.RequireAll(m => m.IsAvailable);
});
```

## Pre-Operation Actions
**See Also:** [Actions](xref:actions)

These actions run before every operation. They can be used to make changes to the parsed body before the operation is run.

### Example
```csharp
api.PostCreate(route => {
    route.SetValue(m => m.LuckyNumber, (c) => 4);
});
```

## Operations
**See Also:** [Operations](xref:operations)

Every API route can have at most one <xref:Seltzr.Operations.IOperation`2> registered. An operation transforms the database/backend in some way, usually that means by creating, updating, or deleting models. When using an ORM-backed Seltzr, these are usually added for you, but they can also be set manually. Every operation returns a dataset that becomes the new dataset for the Result Writer. If there is no operation, the Result Writer will write out the existing dataset.

> [!NOTE]
> Regardless of whether or not an operation is provided, pre- and post-operation actions will always be run.

### Example
This example manually sets up a route to create new models using Entity Framework from the parsed request body.
```csharp
api.SetupPost(route => {
    route.UseOperation(new CreateOperation<MyModel, MyContext>());
});
```


## Post-Operation Actions
**See Also:** [Actions](xref:actions)

These actions run after every operation. They can be used for logging, notifying the rest of the app that data has changed, or modifying the response body.

### Example
```csharp
api.PostCreate(route => {
    route.After((c, d) => c.HttpResponse.Cookies.Append("cookie", "monster"));
});
```

## Writing the Result
**See Also:** <xref:result-writers>

Finally, the output result is written to the response body and sent off to the user via a [Result Writer](xref:Seltzr.Results.IResultWriter`2). Typically the Writer just writes the model itself, but the response can also be wrapped in a [Response](xref:Seltzr.Responses.Response`1) object (see <xref:response-wrapping>).


### Example
The following example writes the final dataset wrapped in a <xref:Seltzr.Responses.BasicResponse`1>
```csharp
api.Get(route => {
    route.WrapResponse().WriteJson();
});
```

> [!WARNING]
> There can only be one result writer for each route. For a context-dependent writer that looks at the `Accept` or `Content-Type` header, or a query parameter, look at the <xref:Seltzr.Results.AcceptDependentResultWriter`2>, <xref:Seltzr.Results.HeaderDependentResultWriter`2>, and <xref:Seltzr.Results.QueryDependentResultWriter`2> classes.

## Exception Handlers
**See Also:** [Exception Handlers](xref:exception-handlers)

An [Exception Handler](xref:Seltzr.ExceptionHandlers.IExceptionHandler) is run, as its name suggests, whenever an exception is thrown. They can be used to customize or log error messages, but if no Exception Handlers are provided, regular ASP.NET exception middleware can be used instead. For most cases, the <xref:Seltzr.ExceptionHandlers.SimpleExceptionHandler> will work well.

### Example
The following example adds a <xref:Seltzr.ExceptionHandlers.SimpleExceptionHandler> using the helper method `CatchExceptions`, but also logs when parsing errors occur.
```csharp
api.PostCreate(route => {
    route.Catch<ParsingFailedException>(e => MyLogger.LogError(e))
        .CatchExceptions();
});
```