﻿// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilder.RequestMethods.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Options.Builder {
	using System;

	/// <content>
	///     Builder for <see cref="SeltzrOptions{TModel, TUser}" />
	/// </content>
	public partial class SeltzrOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Indicates that this route accepts DELETE requests
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> CanDelete() => this.AddRequestMethod("DELETE");

		/// <summary>
		///     Indicates that this route accepts GET requests
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> CanGet() => this.AddRequestMethod("GET");

		/// <summary>
		///     Indicates that this route accepts PATCH requests
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> CanPatch() => this.AddRequestMethod("PATCH");

		/// <summary>
		///     Indicates that this route accepts POST requests
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> CanPost() => this.AddRequestMethod("POST");

		/// <summary>
		///     Indicates that this route accepts PUT requests
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> CanPut() => this.AddRequestMethod("PUT");

		/// <summary>
		///     Sets up an anonymous GET request for the given route pattern, clearing auth providers, body parsers, and any
		///     operation.
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupAnonymousGet(
			string routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) {
			return this.SetupGet(
				routePattern,
				o => {
					o.ClearAuthProviders();
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets up an anonymous GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupAnonymousGet(
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) =>
			this.SetupAnonymousGet(string.Empty, optionsHandler);

		/// <summary>
		///     Sets up an anonymous GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupAnonymousGet() => this.SetupAnonymousGet(null);

		/// <summary>
		///     Sets up a DELETE request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupDelete(
			string routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) {
			SeltzrOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.ClearBodyParsers();
			OptionsBuilder.CanDelete();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a DELETE request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupDelete(
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) =>
			this.SetupDelete(string.Empty, optionsHandler);

		/// <summary>
		///     Sets up a DELETE request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupDelete() => this.SetupDelete(null);

		/// <summary>
		///     Sets up a GET request for the given route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupGet(
			string? routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) {
			SeltzrOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanGet();
			OptionsBuilder.ClearBodyParsers();
			OptionsBuilder.ClearOperation();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupGet(Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) =>
			this.SetupGet(string.Empty, optionsHandler);

		/// <summary>
		///     Sets up a GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupGet() => this.SetupGet(null);

		/// <summary>
		///     Sets up a PATCH request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupPatch(
			string routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) {
			SeltzrOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanPatch();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a PATCH request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupPatch(
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) =>
			this.SetupPatch(string.Empty, optionsHandler);

		/// <summary>
		///     Sets up a PATCH request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupPatch() => this.SetupPatch(null);

		/// <summary>
		///     Sets up a POST request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupPost(
			string routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) {
			SeltzrOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanPost();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a POST request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupPost(
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) =>
			this.SetupPost(string.Empty, optionsHandler);

		/// <summary>
		///     Sets up a POST request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupPost() => this.SetupPost(null);

		/// <summary>
		///     Sets up a PUT request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupPut(
			string routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) {
			SeltzrOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanPut();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a PUT request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupPut(Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler) =>
			this.SetupPut(string.Empty, optionsHandler);

		/// <summary>
		///     Sets up a PUT request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SetupPut() => this.SetupPut(null);

		/// <summary>
		///     Indicates that this route accepts the given request methods
		/// </summary>
		/// <param name="methods">The request methods to accept</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> UseRequestMethods(params string[] methods) {
			this.ClearRequestMethods();
			foreach (string Method in methods) this.AddRequestMethod(Method);
			return this;
		}
	}
}