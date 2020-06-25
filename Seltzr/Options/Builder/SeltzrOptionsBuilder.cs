// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilder.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Options.Builder {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	using Microsoft.AspNetCore.Builder;

	using Seltzr.Actions;
	using Seltzr.Auth;
	using Seltzr.Conditions;
	using Seltzr.Exceptions;
	using Seltzr.Filters;
	using Seltzr.Models;
	using Seltzr.Operations;
	using Seltzr.Parsers;
	using Seltzr.Responses;
	using Seltzr.Results;

	/// <summary>
	///     Builder for <see cref="SeltzrOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class SeltzrOptionsBuilder<TModel, TUser> : SeltzrOptionsBuilderBase
		where TModel : class where TUser : class {
		/// <summary>
		///     The children options being built off of this one
		/// </summary>
		private readonly List<SeltzrOptionsBuilder<TModel, TUser>> Children =
			new List<SeltzrOptionsBuilder<TModel, TUser>>();

		/// <summary>
		///     The options object that's being built
		/// </summary>
		private readonly SeltzrOptions<TModel, TUser> Options;

		/// <summary>
		///     Initializes a new instance of the <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="existing">Starting options for this builder</param>
		public SeltzrOptionsBuilder(SeltzrOptions<TModel, TUser>? existing = null) =>
			this.Options = existing ?? new SeltzrOptions<TModel, TUser>();

		/// <summary>
		///     Initializes a new instance of the <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="baseRoute">The base route for these options</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		public SeltzrOptionsBuilder(string baseRoute, Action<IEndpointConventionBuilder>? routeOptionsHandler)
			: this() {
			this.Options.RoutePattern = SeltzrOptionsBuilderBase.FixRoute(baseRoute);
			this.Options.RouteOptionsHandler = routeOptionsHandler;
		}

		/// <summary>
		///     Gets the route pattern for this builder
		/// </summary>
		public string RoutePattern => this.Options.RoutePattern;

		/// <summary>
		///     Sets whether or not to accept arrays of <typeparamref name="TModel" /> as the request body
		/// </summary>
		/// <param name="accept">A value indicating whether or not to accept arrays as the request body</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AcceptArrays(bool accept = true) {
			this.Options.ParserOptions.ParseArrays = accept;
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route
		/// </summary>
		/// <param name="authProvider">The auth provider to add</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddAuthProvider(IAuthProvider<TModel, TUser> authProvider) {
			if (this.Options.AuthProviders == null)
				this.Options.AuthProviders = new List<IAuthProvider<TModel, TUser>>();
			this.Options.AuthProviders.Add(authProvider);
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route
		/// </summary>
		/// <typeparam name="TProvider">The type of auth provider to add</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddAuthProvider<TProvider>()
			where TProvider : IAuthProvider<TModel, TUser>, new() {
			this.AddAuthProvider(new TProvider());
			return this;
		}

		/// <summary>
		///     Adds a body parser to this route
		/// </summary>
		/// <param name="bodyParser">The body parser to add</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddBodyParser(IBodyParser<TModel> bodyParser) {
			if (this.Options.BodyParsers == null) this.Options.BodyParsers = new List<IBodyParser<TModel>>();
			this.Options.BodyParsers.Add(bodyParser);
			return this;
		}

		/// <summary>
		///     Adds a pre-operation action to this route
		/// </summary>
		/// <param name="action">The pre-op action to add</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddPreOpAction(IPreOpAction<TModel, TUser> action) {
			this.Options.PreOpActions.Add(action);
			return this;
		}

		/// <summary>
		///     Adds a pre-operation action to this route
		/// </summary>
		/// <typeparam name="TAction">The type of pre-op action to add</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddPreOpAction<TAction>() where TAction: IPreOpAction<TModel, TUser>, new() {
			this.AddPreOpAction(new TAction());
			return this;
		}

		/// <summary>
		///     Adds a post-operation action to this route
		/// </summary>
		/// <typeparam name="TAction">The type of post-op action to add</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>
		///		This action will be run after every operation and can be used to log data, notify other parts of the app, or modify the HTTP response
		///		<para>
		///			Note that if you would like to omit/include properties from the API response or set response parameters, use the <see cref="SeltzrOptionsBuilder{TModel, TUser}.Omit(Expression{Func{TModel, object}})" />, <see cref="SeltzrOptionsBuilder{TModel, TUser}.Include(Expression{Func{TModel, object}})" />, and <see cref="SeltzrOptionsBuilder{TModel, TUser}.WriteResponseValue(string, object)" /> methods respectively.
		///		</para>
		/// </remarks>
		public SeltzrOptionsBuilder<TModel, TUser> AddPostOpAction<TAction>() where TAction : IPostOpAction<TModel, TUser>, new() {
			this.AddPostOpAction(new TAction());
			return this;
		}

		/// <summary>
		///     Adds a post-operation action to this route
		/// </summary>
		/// <param name="action">The post-op action to add</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>
		///		This action will be run after every operation and can be used to log data, notify other parts of the app, or modify the HTTP response
		///		<para>
		///			Note that if you would like to omit/include properties from the API response or set response parameters, use the <see cref="SeltzrOptionsBuilder{TModel, TUser}.Omit(Expression{Func{TModel, object}})" />, <see cref="SeltzrOptionsBuilder{TModel, TUser}.Include(Expression{Func{TModel, object}})" />, and <see cref="SeltzrOptionsBuilder{TModel, TUser}.WriteResponseValue(string, object)" /> methods respectively.
		///		</para>
		/// </remarks>
		public SeltzrOptionsBuilder<TModel, TUser> AddPostOpAction(IPostOpAction<TModel, TUser> action) {
			this.Options.PostOpActions.Add(action);
			return this;
		}

		/// <summary>
		///     Adds a new body parser to this route
		/// </summary>
		/// <typeparam name="TParser">The type of body parser to add</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddBodyParser<TParser>()
			where TParser : IBodyParser<TModel>, new() {
			this.AddBodyParser(new TParser());
			return this;
		}

		/// <summary>
		///     Adds a condition to this route, which much be met for the request to succeed
		/// </summary>
		/// <param name="condition">The condition to add</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddCondition(ICondition<TModel, TUser> condition) {
			this.Options.Conditions.Add(condition);
			return this;
		}

		/// <summary>
		///     Adds a condition to this route, which much be met for the request to succeed
		/// </summary>
		/// <typeparam name="TCondition">The type of the condition to add</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddCondition<TCondition>() where TCondition : ICondition<TModel, TUser>, new() {
			return this.AddCondition(new TCondition());
		}

		/// <summary>
		///     Adds a filter to this route, which will filter the dataset retrieved
		/// </summary>
		/// <param name="filter">The filter to add</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddFilter(IFilter<TModel, TUser> filter) {
			this.Options.Filters.Add(filter);
			return this;
		}

		/// <summary>
		///     Adds a filter to this route, which will filter the dataset retrieved
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter to add</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddFilter<TFilter>() where TFilter : IFilter<TModel, TUser>, new() {
			return this.AddFilter(new TFilter());
		}

		/// <summary>
		///     Adds a request method that this route supports
		/// </summary>
		/// <param name="requestMethod">The request method to add</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> AddRequestMethod(string requestMethod) {
			if (this.Options.RequestMethods == null) this.Options.RequestMethods = new HashSet<string>();
			this.Options.RequestMethods.Add(requestMethod);
			return this;
		}

		/// <summary>
		///     Builds these options and all of this builder's child options. Not intended for application use.
		/// </summary>
		/// <returns>
		///     A list of all of the options created by this builder, including its own, in the order they were added, keyed
		///     to the route it's for
		/// </returns>
		public Dictionary<string, List<SeltzrOptions<TModel, TUser>>> BuildAll() {
			// create options
			// this is not the prettiest thing i've ever written
			List<SeltzrOptions<TModel, TUser>> MyOptions = new List<SeltzrOptions<TModel, TUser>> { this.Options };
			Dictionary<string, List<SeltzrOptions<TModel, TUser>>> AllOptions =
				new Dictionary<string, List<SeltzrOptions<TModel, TUser>>> {
					                                                           {
						                                                           this.Options.RoutePattern,
						                                                           MyOptions
					                                                           }
				                                                           };
			foreach (SeltzrOptionsBuilder<TModel, TUser> Builder in this.Children)
			foreach ((string Key, List<SeltzrOptions<TModel, TUser>> OptionsList) in Builder.BuildAll())
				if (AllOptions.ContainsKey(Key)) AllOptions[Key].AddRange(OptionsList);
				else AllOptions.Add(Key, OptionsList);

			return AllOptions;
		}

		/// <summary>
		///     Clears all of the auth providers registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearAuthProviders() {
			this.Options.AuthProviders = null;
			return this;
		}

		/// <summary>
		///     Clears all of the pre-operation actions registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearPreOpActions() {
			this.Options.PreOpActions.Clear();
			return this;
		}

		/// <summary>
		///     Clears all of the post-operation actions registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearPostOpActions() {
			this.Options.PostOpActions.Clear();
			return this;
		}


		/// <summary>
		///     Clears all of the body parsers registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearBodyParsers() {
			this.Options.BodyParsers = null;
			return this;
		}

		/// <summary>
		///     Clears all of the conditions registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearConditions() {
			this.Options.Conditions.Clear();
			return this;
		}

		/// <summary>
		///     Clears all of the exception handlers registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearExceptionHandlers() {
			this.Options.ExceptionHandlers.Clear();
			return this;
		}

		/// <summary>
		///     Clears all of the filters registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearFilters() {
			this.Options.Filters.Clear();
			return this;
		}

		/// <summary>
		///     Clears the operation registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearOperation() {
			this.Options.Operation = null;
			return this;
		}

		/// <summary>
		///     Clears all of the request methods registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearRequestMethods() {
			this.Options.RequestMethods = null;
			return this;
		}

		/// <summary>
		///     Clears the result writer registered for this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ClearResultWriter() {
			this.Options.ResultWriter = null;
			return this;
		}

		/// <summary>
		///     Creates a child instance of the <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> type sharing the given base
		///     options. When overriden in a derived class, this method can be used to ensure that the entire tree of
		///     <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> objects share the same derived type
		/// </summary>
		/// <param name="baseOptions">The base options for the new instance</param>
		/// <returns>The new <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> instance</returns>
		public virtual SeltzrOptionsBuilder<TModel, TUser> CreateChild(SeltzrOptions<TModel, TUser>? baseOptions) =>
			new SeltzrOptionsBuilder<TModel, TUser>(baseOptions);

		/// <summary>
		///     Sets a default value for a property of the <typeparamref name="TModel" /> parsed from the request body
		/// </summary>
		/// <param name="property">The property to set the default for</param>
		/// <param name="defaultValue">
		///     A delegate that will retrieve the default value for the given <paramref name="property" />
		/// </param>
		/// <typeparam name="TProperty">The type of the property for to set the default for</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Default<TProperty>(
			PropertyInfo property,
			Func<TProperty> defaultValue) {
			// double lambda looks silly but we can't cast Func<TProperty> to Func<object> without making it constrained to reference types
			this.Options.ParserOptions.DefaultPropertyValues.Add(property, () => defaultValue());
			return this;
		}

		/// <summary>
		///     Sets a default value for a property of the <typeparamref name="TModel" /> parsed from the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to set a default for</param>
		/// <param name="defaultValue">
		///     A delegate that will retrieve the default value for the property defined by
		///     <paramref name="propertyExpression" />
		/// </param>
		/// <typeparam name="TProperty">The type of the property for to set the default for</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Default<TProperty>(
			Expression<Func<TModel, TProperty>> propertyExpression,
			Func<TProperty> defaultValue) {
			PropertyInfo Info = SeltzrOptionsBuilderBase.ExtractProperty(propertyExpression);

			// double lambda looks silly but we can't cast Func<TProperty> to Func<object> without making it constrained to reference types
			// todo: would there ever be a non-reference type in a model though?
			this.Options.ParserOptions.DefaultPropertyValues.Add(Info, () => defaultValue());
			return this;
		}

		/// <summary>
		///     Sets a default value for a property of the <typeparamref name="TModel" /> parsed from the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to set a default for</param>
		/// <param name="defaultValue">The default value for the property defined by <paramref name="propertyExpression" /></param>
		/// <typeparam name="TProperty">The type of the property for to set the default for</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Default<TProperty>(
			Expression<Func<TModel, TProperty>> propertyExpression,
			TProperty defaultValue) {
			PropertyInfo Info = SeltzrOptionsBuilderBase.ExtractProperty(propertyExpression);
			this.Options.ParserOptions.DefaultPropertyValues.Add(Info, () => defaultValue);
			return this;
		}

		/// <summary>
		///     Maps another route to the same route pattern
		/// </summary>
		/// <returns>The new <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object</returns>
		/// <remarks>
		///     This method works just like
		///     <see cref="MapRoute(string, Action{SeltzrOptionsBuilder{TModel, TUser}}, Action{IEndpointConventionBuilder})" />
		///     , but returns the newly created <see cref="SeltzrOptionsBuilder{TModel, TUser}" />, rather than the one used to
		///     create the route.
		/// </remarks>
		public SeltzrOptionsBuilder<TModel, TUser> FlatMap() => this.FlatMap(this.Options.RoutePattern, null);

		/// <summary>
		///     Maps a route to a route pattern
		/// </summary>
		/// <param name="pattern">The pattern to match with</param>
		/// <returns>The new <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object</returns>
		/// <remarks>
		///     This method works just like
		///     <see cref="MapRoute(string, Action{SeltzrOptionsBuilder{TModel, TUser}}, Action{IEndpointConventionBuilder})" />
		///     , but returns the newly created <see cref="SeltzrOptionsBuilder{TModel, TUser}" />, rather than the one used to
		///     create the route.
		/// </remarks>
		public SeltzrOptionsBuilder<TModel, TUser> FlatMap(string? pattern) => this.FlatMap(pattern, null);

		/// <summary>
		///     Maps another route to the same route pattern
		/// </summary>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		/// <returns>The new <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object</returns>
		/// <remarks>
		///     This method works just like
		///     <see cref="MapRoute(string, Action{SeltzrOptionsBuilder{TModel, TUser}}, Action{IEndpointConventionBuilder})" />
		///     , but returns the newly created <see cref="SeltzrOptionsBuilder{TModel, TUser}" />, rather than the one used to
		///     create the route.
		/// </remarks>
		public SeltzrOptionsBuilder<TModel, TUser> FlatMap(Action<IEndpointConventionBuilder>? routeOptionsHandler) =>
			this.FlatMap(this.Options.RoutePattern, routeOptionsHandler);

		/// <summary>
		///     Maps a route to a route pattern
		/// </summary>
		/// <param name="pattern">The pattern to match with</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		/// <returns>The new <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object</returns>
		/// <remarks>
		///     This method works just like
		///     <see cref="MapRoute(string, Action{SeltzrOptionsBuilder{TModel, TUser}}, Action{IEndpointConventionBuilder})" />
		///     , but returns the newly created <see cref="SeltzrOptionsBuilder{TModel, TUser}" />, rather than the one used to
		///     create the route.
		/// </remarks>
		public SeltzrOptionsBuilder<TModel, TUser> FlatMap(
			string? pattern,
			Action<IEndpointConventionBuilder>? routeOptionsHandler) {
			// create a copy of the options and use that for the new route
			SeltzrOptions<TModel, TUser> Copy = this.Options.Copy();
			Copy.RoutePattern += SeltzrOptionsBuilderBase.FixRoute(pattern);
			if (routeOptionsHandler != null) Copy.RouteOptionsHandler = routeOptionsHandler;
			SeltzrOptionsBuilder<TModel, TUser> Child = this.CreateChild(Copy);
			this.Children.Add(Child);

			return Child;
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will not be parsed from the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to be ignored</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Ignore(Expression<Func<TModel, object>> propertyExpression) {
			return this.Ignore(SeltzrOptionsBuilderBase.ExtractProperty(propertyExpression));
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will not be parsed from the request body
		/// </summary>
		/// <param name="property">The property to be ignored</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Ignore(PropertyInfo property) {
			// todo: add to other methods
			if (property.DeclaringType != null && typeof(TModel) != property.DeclaringType
			                                   && !property.DeclaringType.IsSubclassOf(typeof(TModel)))
				throw new OptionsException("Cannot ignore a property that doesn't belong to the model class");
			this.Options.ParserOptions.IgnoredParseProperties.Add(property);
			this.Options.ParserOptions.RequiredParseProperties?.Remove(property);
			return this;
		}

		/// <summary>
		///     Ensures that all properties of the <typeparamref name="TModel" /> will not be parsed from the request body
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> IgnoreAll() {
			this.Options.ParserOptions.IgnoredParseProperties.AddRange(
				typeof(TModel).GetProperties().Where(p => p.CanWrite));
			this.Options.ParserOptions.RequiredParseProperties?.Clear();
			return this;
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to be included</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Include(Expression<Func<TModel, object>> propertyExpression) =>
			this.Include(SeltzrOptionsBuilderBase.ExtractProperty(propertyExpression));

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <param name="property">The property to be ignored</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Include(PropertyInfo property) {
			if (property.DeclaringType != null && typeof(TModel) != property.DeclaringType
			                                   && !property.DeclaringType.IsSubclassOf(typeof(TModel)))
				throw new OptionsException("Cannot include a property that doesn't belong to the model class");

			if (this.Options.FormattingOptions.IncludedReturnProperties == null)
				this.Options.FormattingOptions.IncludedReturnProperties = new List<PropertyInfo>();

			this.Options.FormattingOptions.IncludedReturnProperties.Add(property);
			return this;
		}

		/// <summary>
		///     Ensures that all properties of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> IncludeAll() {
			this.Options.FormattingOptions.IncludedReturnProperties = null;
			return this;
		}

		/// <summary>
		///     Maps a route to a route pattern
		/// </summary>
		/// <param name="pattern">The pattern to match with</param>
		/// <param name="optionsHandler">The options for this route</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> MapRoute(
			string pattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler) {
			optionsHandler(this.FlatMap(pattern));
			return this;
		}

		/// <summary>
		///     Maps another route to the same route pattern
		/// </summary>
		/// <param name="optionsHandler">The options for this route</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> MapRoute(
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler) {
			optionsHandler(this.FlatMap());
			return this;
		}

		/// <summary>
		///     Maps another route to the same route pattern
		/// </summary>
		/// <param name="optionsHandler">The options for this route</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> MapRoute(
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler,
			Action<IEndpointConventionBuilder> routeOptionsHandler) {
			optionsHandler(this.FlatMap(routeOptionsHandler));
			return this;
		}

		/// <summary>
		///     Maps a route to a route pattern
		/// </summary>
		/// <param name="pattern">The pattern to match with</param>
		/// <param name="optionsHandler">The options for this route</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> MapRoute(
			string pattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler,
			Action<IEndpointConventionBuilder> routeOptionsHandler) {
			optionsHandler(this.FlatMap(pattern, routeOptionsHandler));
			return this;
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will not be included in the response body
		/// </summary>
		/// <param name="property">The property to be omitted</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Omit(PropertyInfo property) {
			if (this.Options.FormattingOptions.IncludedReturnProperties == null) {
				this.Options.FormattingOptions.IncludedReturnProperties = new List<PropertyInfo>();
				this.Options.FormattingOptions.IncludedReturnProperties.AddRange(typeof(TModel).GetProperties());
			}

			this.Options.FormattingOptions.IncludedReturnProperties!.Remove(property);
			return this;
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will not be included in the response body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to be omitted</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Omit(Expression<Func<TModel, object>> propertyExpression) =>
			this.Omit(SeltzrOptionsBuilderBase.ExtractProperty(propertyExpression));

		/// <summary>
		///     Ensures that no properties of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> OmitAll() {
			this.Options.FormattingOptions.IncludedReturnProperties = new List<PropertyInfo>();
			return this;
		}

		/// <summary>
		///     Makes a property of the <typeparamref name="TModel" /> optional in the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to make optional</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> OptionalProperty(Expression<Func<TModel, object>> propertyExpression) =>
			this.OptionalProperty(SeltzrOptionsBuilderBase.ExtractProperty(propertyExpression));

		/// <summary>
		///     Makes a property of the <typeparamref name="TModel" /> optional in the request body
		/// </summary>
		/// <param name="property">The property to make optional</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> OptionalProperty(PropertyInfo property) {
			this.Options.ParserOptions.RequiredParseProperties.Remove(property);
			return this;
		}

		/// <summary>
		///     Makes all properties of the <typeparamref name="TModel" /> optional in the request body
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> OptionalAllProperties() {
			this.Options.ParserOptions.RequiredParseProperties.Clear();
			return this;
		}


		/// <summary>
		///     Requires all properties of the <typeparamref name="TModel" /> to be present in the request body
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireAllProperties() {
			this.Options.ParserOptions.RequiredParseProperties.AddRange(
				typeof(TModel).GetProperties().Where(p => p.CanWrite));
			this.Options.ParserOptions.IgnoredParseProperties?.Clear();
			return this;
		}

		/// <summary>
		///     Requires a property of the <typeparamref name="TModel" /> to be present in the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to require</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireProperty(
			Expression<Func<TModel, object>> propertyExpression) =>
			this.RequireProperty(SeltzrOptionsBuilderBase.ExtractProperty(propertyExpression));

		/// <summary>
		///     Requires a property of the <typeparamref name="TModel" /> to be present in the request body
		/// </summary>
		/// <param name="property">The property to require</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireProperty(PropertyInfo property) {
			this.Options.ParserOptions.RequiredParseProperties.Add(property);
			this.Options.ParserOptions.IgnoredParseProperties?.Remove(property);
			return this;
		}

		/// <summary>
		///     Resets this <see cref="SeltzrOptionsBuilder{TModel, TUser}" />, clearing all lists and resetting all values
		///     except for the route pattern and model provider
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Reset() {
			this.ClearAuthProviders();
			this.ClearBodyParsers();
			this.ClearConditions();
			this.ClearExceptionHandlers();
			this.ClearFilters();
			this.ClearOperation();
			this.ClearRequestMethods();
			this.ClearResultWriter();
			return this;
		}

		/// <summary>
		///     Sets whether or not to write the result as a <typeparamref name="TModel" /> instead of as a collection of
		///     <typeparamref name="TModel" /> if there is only a single element in the result array
		/// </summary>
		/// <param name="strip">
		///     A value indicating whether or not to strip the array from a result body if there is only a single
		///     element in the result array
		/// </param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> StripArrayIfSingleResult(bool strip = true) {
			this.Options.FormattingOptions.StripArrayIfSingleElement = strip;
			return this;
		}

		/// <summary>
		///     Sets the model provider to use for this route
		/// </summary>
		/// <param name="modelProvider">The model provider to use</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> UseModelProvider(IModelProvider<TModel, TUser> modelProvider) {
			this.Options.ModelProvider = modelProvider;
			return this;
		}

		/// <summary>
		///     Sets the model provider to use for this route
		/// </summary>
		/// <typeparam name="TProvider">The type of the model provider to use</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> UseModelProvider<TProvider>() where TProvider : IModelProvider<TModel, TUser>, new() {
			return this.UseModelProvider(new TProvider());
		}

		/// <summary>
		///     Sets the operation to use for this route, like create, update, or delete
		/// </summary>
		/// <param name="operation">The operation to use</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> UseOperation(IOperation<TModel, TUser> operation) {
			this.Options.Operation = operation;
			return this;
		}

		/// <summary>
		///     Sets the operation to use for this route, like create, update, or delete
		/// </summary>
		/// <typeparam name="TOperation">The type of the operation to use</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> UseOperation<TOperation>() where TOperation : IOperation<TModel, TUser>, new() {
			return this.UseOperation(new TOperation());
		}

		/// <summary>
		///     Sets the result writer to use for this route
		/// </summary>
		/// <param name="resultWriter">The result writer to use</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> UseResultWriter(IResultWriter<TModel, TUser> resultWriter) {
			this.Options.ResultWriter = resultWriter;
			return this;
		}

		/// <summary>
		///     Sets the result writer to use for this route
		/// </summary>
		/// <typeparam name="TWriter">The type of result writer to use</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> UseResultWriter<TWriter>()
			where TWriter : IResultWriter<TModel, TUser>, new() {
			this.Options.ResultWriter = new TWriter();
			return this;
		}

		/// <summary>
		///     Wraps all API responses in an instance of the given type
		/// </summary>
		/// <typeparam name="TResponse">The type of the response wrapper</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> WrapResponse<TResponse>()
			where TResponse : Response<TModel> {
			this.Options.ResponseType = typeof(TResponse);
			return this;
		}

		/// <summary>
		///     Wraps all API responses in an instance of <see cref="BasicResponse{TModel}"/>
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> WrapResponse() {
			return this.WrapResponse<BasicResponse<TModel>>();
		}
	}
}