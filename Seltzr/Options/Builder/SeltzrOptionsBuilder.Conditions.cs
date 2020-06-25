// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilder.Conditions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Options.Builder {
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using Seltzr.Conditions;
	using Seltzr.Context;
	using Seltzr.Filters;
	using Seltzr.ParameterRetrievers;

	/// <summary>
	///     Builder for <see cref="SeltzrOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class SeltzrOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Require(
			Func<IApiContext<TModel, TUser>, IQueryable<TModel>, bool> condition,
			string? failureMessage = null) {
			this.RequireAsync(async (c, d) => condition(c, d), failureMessage);
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Require(Func<IQueryable<TModel>, bool> condition, string? failureMessage = null) {
			this.RequireAsync(async d => condition(d), failureMessage);
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure that every element in the dataset meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the dataset's elements meet a condition</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireAll(Func<TModel, bool> condition, string? failureMessage = null) {
			this.Require(d => d.All(condition), failureMessage);
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the parsed body meets the given condition. If there is no parsed body, this condition will always pass.
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the parsed body meets a condition</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireInput(Func<TModel[]?, bool> condition, string? failureMessage = null) {
			this.Require((c, d) => c.Parsed == null || condition(c.Parsed?.Select(p => p.ParsedModel).ToArray()), failureMessage ?? "Request body did not meet required condition");
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the parsed body meets the given condition. If there is no parsed body, this condition will always pass.
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the parsed body meets a condition</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireAllInput(Func<TModel, bool> condition, string? failureMessage = null) {
			this.RequireInput(p => p?.All(condition) ?? true, failureMessage ?? "Request body did not meet required condition");
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the parsed body meets the given condition. If there is no parsed body, this condition will always pass.
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the parsed body meets a condition</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireInputAsync(Func<TModel[], Task<bool>> condition, string? failureMessage = null) {
			this.RequireAsync(async (c, d) => c.Parsed == null || await condition(c.Parsed.Select(p => p.ParsedModel).ToArray()), failureMessage ?? "Request body did not meet required condition");
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireAsync(
			Func<IApiContext<TModel, TUser>, IQueryable<TModel>, Task<bool>> condition, string? failureMessage = null) {
			this.AddCondition(new DelegateCondition<TModel, TUser>(condition, failureMessage));
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireAsync(Func<IQueryable<TModel>, Task<bool>> condition, string? failureMessage = null) {
			this.RequireAsync((c, d) => condition(d), failureMessage);
			return this;
		}

		/// <summary>
		///     Ensures that there are at least a certain number of elements in the dataset
		/// </summary>
		/// <param name="count">The number of elements that should be in the dataset at a minimum</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireAtLeast(int count, string? failureMessage = null) => this.Require(d => d.Count() >= count, failureMessage ?? $"Request must match at least {count} elements");

		/// <summary>
		///     Ensures that there are exactly a certain number of elements in the dataset
		/// </summary>
		/// <param name="count">The number of elements that should be in the dataset</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireExactly(int count, string? failureMessage = null) =>
			this.Require(d => d.Count() == count, failureMessage ?? $"Request must match exactly {count} elements");


		/// <summary>
		///     Ensures that there are at least a certain number of elements in the parsed body
		/// </summary>
		/// <param name="count">The number of elements that should be in the dataset at a minimum</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireInputHasAtLeast(int count, string? failureMessage = null) => this.RequireInput(p => p.Length >= count, failureMessage ?? $"At least {count} element(s) must be provided in the request body");

		/// <summary>
		///     Ensures that there are exactly a certain number of elements in the parsed dataset
		/// </summary>
		/// <param name="count">The number of elements that should be in the dataset</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireInputHasExactly(int count, string? failureMessage = null) =>
			this.RequireInput(p => p == null || p.Length == count, failureMessage ?? $"{count} element(s) must be provided in the request body");

		/// <summary>
		///     Ensures that there is exactly one element in the dataset
		/// </summary>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireExactlyOne(string? failureMessage = null) => this.RequireExactly(1, failureMessage ?? "Request must match only one element");

		/// <summary>
		///     Ensures that there is at least one element in the dataset
		/// </summary>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireNonEmpty(string? failureMessage = null) => this.RequireAtLeast(1, failureMessage ?? "Request matched no elements");

		/// <summary>
		///     Ensures that there is exactly one element in the parsed body
		/// </summary>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireExactlyOneInput(string? failureMessage = null) => this.RequireInputHasExactly(1, failureMessage ?? "One element must be provided in the request body");

		/// <summary>
		///     Ensures that there is at least one element in the parsed body
		/// </summary>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireNonEmptyInput(string? failureMessage = null) => this.RequireInputHasAtLeast(1, failureMessage ?? "At least one element must be provided in the request body");

		/// <summary>
		///		Ensures that a route value meets a given condition
		/// </summary>
		/// <typeparam name="T">The type to parse the route value to</typeparam>
		/// <param name="routeValueName">The route value to set a condition on</param>
		/// <param name="condition">The condition which, when passed the parsed parameter, will return a boolean value indicating whether or not the condition has been met</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireRoute<T>(
			string routeValueName,
			Func<T, bool> condition,
			string? failureMessage = null) =>
			this.RequireParameter(new RouteValueRetriever(routeValueName), condition, failureMessage, false);

		/// <summary>
		///		Ensures that a route value meets a given condition. If the route value is not present, the condition is not checked
		/// </summary>
		/// <typeparam name="T">The type to parse the route value to</typeparam>
		/// <param name="routeValueName">The route value to set a condition on</param>
		/// <param name="condition">The condition which, when passed the parsed parameter, will return a boolean value indicating whether or not the condition has been met</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireRouteOpt<T>(
			string routeValueName,
			Func<T, bool> condition,
			string? failureMessage = null) =>
			this.RequireParameter(new RouteValueRetriever(routeValueName), condition, failureMessage, true);

		/// <summary>
		///		Ensures that a query parameter meets a given condition
		/// </summary>
		/// <typeparam name="T">The type to parse the query parameter to</typeparam>
		/// <param name="queryParameterName">The query parameter to set a condition on</param>
		/// <param name="condition">The condition which, when passed the parsed parameter, will return a boolean value indicating whether or not the condition has been met</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireQuery<T>(
			string queryParameterName,
			Func<T, bool> condition,
			string? failureMessage = null) =>
			this.RequireParameter(new QueryParameterRetriever(queryParameterName), condition, failureMessage, false);

		/// <summary>
		///		Ensures that a query parameter meets a given condition. If the query parameter is not present, the condition is not checked
		/// </summary>
		/// <typeparam name="T">The type to parse the query parameter to</typeparam>
		/// <param name="queryParameterName">The query parameter to set a condition on</param>
		/// <param name="condition">The condition which, when passed the parsed parameter, will return a boolean value indicating whether or not the condition has been met</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireQueryOpt<T>(
			string queryParameterName,
			Func<T, bool> condition,
			string? failureMessage = null) =>
			this.RequireParameter(new QueryParameterRetriever(queryParameterName), condition, failureMessage, true);

		/// <summary>
		///		Ensures that a request parameter meets a given condition
		/// </summary>
		/// <typeparam name="T">The type to parse the request parameter to</typeparam>
		/// <param name="parameter">The parameter to set a condition on</param>
		/// <param name="condition">The condition which, when passed the parsed parameter, will return a boolean value indicating whether or not the condition has been met</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireParameter<T>(
			ParameterRetriever parameter,
			Func<T, bool> condition,
			string? failureMessage = null) =>
			this.RequireParameter(parameter, condition, failureMessage, false);

		/// <summary>
		///		Ensures that a request parameter meets a given condition. If the request parameter is not present, the condition is not checked
		/// </summary>
		/// <typeparam name="T">The type to parse the request parameter to</typeparam>
		/// <param name="parameter">The parameter to set a condition on</param>
		/// <param name="condition">The condition which, when passed the parsed parameter, will return a boolean value indicating whether or not the condition has been met</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> RequireParameterOpt<T>(
			ParameterRetriever parameter,
			Func<T, bool> condition,
			string? failureMessage = null) =>
			this.RequireParameter(parameter, condition, failureMessage, true);

		/// <summary>
		///		Ensures that a request parameter meets a given condition
		/// </summary>
		/// <typeparam name="T">The type to parse the request parameter to</typeparam>
		/// <param name="parameter">The parameter to set a condition on</param>
		/// <param name="condition">The condition which, when passed the parsed parameter, will return a boolean value indicating whether or not the condition has been met</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		/// <param name="optional">
		///     <see langword="true" /> to only check the condition if the request parameter is present,
		///     <see langword="false" /> to always throw if the parameter is not present
		/// </param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		private SeltzrOptionsBuilder<TModel, TUser> RequireParameter<T>(
			ParameterRetriever parameter,
			Func<T, bool> condition,
			string? failureMessage,
			bool optional) {
			Type ParamType = typeof(T);
			return this.Require(
				(c, d) => {
					string? ParameterValue = parameter.GetValue(c.Request);
					if (ParameterValue == null) return optional;

					T Value = (T)ParameterResolver.ParseParameter(ParameterValue, ParamType)!;
					return condition(Value);
				}, failureMessage);
		}
	}
}