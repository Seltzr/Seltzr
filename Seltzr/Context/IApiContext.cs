// -----------------------------------------------------------------------
// <copyright file="IApiContext.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Context {
	using System;

	using Microsoft.AspNetCore.Http;

	using Seltzr.Parsers;
	using Seltzr.Responses;

	/// <summary>
	///     Context for an API request.
	/// </summary>
	/// <typeparam name="TModel">The type of model being managed by the API</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	/// <remarks>
	///		The API Context lives for the duration of a single route execution and contains a number of useful properties which are passed to many steps of the request flow. The API Context provides access to the parsed request body, the response wrapper, and a scoped service provider to Auth Providers, Conditions, Filters, Model Providers, Operations, Parsers, and Result Writers.
	/// </remarks>
	public interface IApiContext<TModel, out TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Gets the current <see cref="HttpContext" /> for this request
		/// </summary>
		public HttpContext HttpContext { get; }

		/// <summary>
		///     Gets the current HTTP response context. Shortcut to <see cref="Microsoft.AspNetCore.Http.HttpContext.Response" />
		/// </summary>
		public HttpResponse HttpResponse { get; }

		/// <summary>
		///     Gets the models that have been parsed by the body parser. This may be null if the body has not been parsed yet or
		///     there are no body parsers registered for this route.
		/// </summary>
		public IParseResult<TModel>[]? Parsed { get; }

		/// <summary>
		///     Gets the current request context. Shortcut to <see cref="HttpContext.Request" />
		/// </summary>
		public HttpRequest Request { get; }

		/// <summary>
		///     Gets the response for this API call. If this is null, the model itself will be serialized instead.
		/// </summary>
		public IResponse<TModel>? Response { get; }

		/// <summary>
		///     Gets a service provider for this API context
		/// </summary>
		public IServiceProvider Services { get; }

		/// <summary>
		///     Gets the authenticated user context for this route. This may be null if authorization has not yet occurred or there
		///     are no auth providers registered for this route.
		/// </summary>
		public TUser? User { get; }
	}
}