// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilder.Filters.cs" company="John Lynch">
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
	using System.Threading.Tasks;

	using Seltzr.Context;
	using Seltzr.Exceptions;
	using Seltzr.Filters;
	using Seltzr.Models;
	using Seltzr.ParameterRetrievers;
	using Seltzr.Responses.Attributes;

	/// <summary>
	///     Builder for <see cref="SeltzrOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class SeltzrOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Retrieves the entire dataset from the <see cref="IQueryable{T}" /> provided by a
		///     <see cref="IModelProvider{TModel,TUser}" />. Enables client-side evaluation for logic impossible to translate to a
		///     query.
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>
		///     Use this method wisely. It can cause a significant performance impact if not only used when absolutely necessary.
		/// </remarks>
		public SeltzrOptionsBuilder<TModel, TUser> FetchModels() => this.Filter(d => d.ToArray().AsQueryable());

		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Filter(
			Func<IApiContext<TModel, TUser>, IQueryable<TModel>, IQueryable<TModel>> filter) {
			this.AddFilter(new DelegateFilter<TModel, TUser>(async (c, d) => filter(c, d)));
			return this;
		}

		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Filter(Func<IQueryable<TModel>, IQueryable<TModel>> filter) {
			this.Filter((c, d) => filter(d));
			return this;
		}

		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterAsync(
			Func<IApiContext<TModel, TUser>, IQueryable<TModel>, Task<IQueryable<TModel>>> filter) {
			this.AddFilter(new DelegateFilter<TModel, TUser>(filter));
			return this;
		}

		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterAsync(
			Func<IQueryable<TModel>, Task<IQueryable<TModel>>> filter) {
			this.FilterAsync((c, d) => filter(d));
			return this;
		}

		/// <summary>
		///     Filters this route's dataset by a parameter value
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="retriever">A function that will get the value of the parameter for a given request</param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByParameter<T>(
			Expression<Func<TModel, T>> property,
			ParameterRetriever retriever,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByParameter(property, retriever, comparer, false);

		/// <summary>
		///     Filters this route's dataset by comparing a given property to a parameter value and comparing the two for equality
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="retriever">A function that will get the value of the parameter for a given request</param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByParameterEqual<T>(
			Expression<Func<TModel, T>> property,
			ParameterRetriever retriever) {
			return this.FilterByParameter(property, retriever, (a, b) => a.Equals(b));
		}

		/// <summary>
		///     Filters this route's dataset by comparing a given property to a parameter value and comparing the two for equality
		///     only if the parameter is present on the request
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="retriever">A function that will get the value of the parameter for a given request</param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByParameterEqualOpt<T>(
			Expression<Func<TModel, T>> property,
			ParameterRetriever retriever) {
			return this.FilterByParameterOpt(property, retriever, (a, b) => a.Equals(b));
		}

		/// <summary>
		///     Filters this route's dataset by a parameter value only if the parameter is present on the request.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="retriever">A function that will get the value of the parameter for a given request</param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByParameterOpt<T>(
			Expression<Func<TModel, T>> property,
			ParameterRetriever retriever,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByParameter(property, retriever, comparer, true);

		/// <summary>
		///     Filters this route's dataset by a query parameter value. The name of the query parameter is inferred to be a
		///     camelCased version of the C# property name.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByQuery<T>(
			Expression<Func<TModel, T>> property,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByQuery(property, SeltzrOptionsBuilderBase.CamelCase(property), comparer);

		// todo: opt methods

		/// <summary>
		///     Filters this route's dataset by a query parameter value
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="queryParameterName">
		///     The name of the query parameter whose value to compare with the value of
		///     <paramref name="property" /> on the dataset's <typeparamref name="TModel" /> objects
		/// </param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByQuery<T>(
			Expression<Func<TModel, T>> property,
			string queryParameterName,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByParameter(property, new QueryParameterRetriever(queryParameterName), comparer);

		/// <summary>
		///     Filters this route's dataset by a query parameter value. The name of the query parameter is inferred to be a
		///     camelCased version of the C# property name.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByQueryEqual<T>(Expression<Func<TModel, T>> property) =>
			this.FilterByQueryEqual(property, SeltzrOptionsBuilderBase.CamelCase(property));

		/// <summary>
		///     Filters this route's dataset by a query parameter value
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="queryParameterName">
		///     The name of the query parameter whose value to compare with the value of
		///     <paramref name="property" /> on the dataset's <typeparamref name="TModel" /> objects
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByQueryEqual<T>(
			Expression<Func<TModel, T>> property,
			string queryParameterName) =>
			this.FilterByParameterEqual(property, new QueryParameterRetriever(queryParameterName));

		/// <summary>
		///     Filters this route's dataset by a query parameter value only if the parameter is present on the request. The name
		///     of the query parameter is inferred to be a
		///     camelCased version of the C# property name.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByQueryEqualOpt<T>(Expression<Func<TModel, T>> property) =>
			this.FilterByQueryEqualOpt(property, SeltzrOptionsBuilderBase.CamelCase(property));

		/// <summary>
		///     Filters this route's dataset by a query parameter value only if the parameter is present on the request
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="queryParameterName">
		///     The name of the query parameter whose value to compare with the value of
		///     <paramref name="property" /> on the dataset's <typeparamref name="TModel" /> objects
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByQueryEqualOpt<T>(
			Expression<Func<TModel, T>> property,
			string queryParameterName) =>
			this.FilterByParameterEqualOpt(property, new QueryParameterRetriever(queryParameterName));

		/// <summary>
		///     Filters this route's dataset by a query parameter value only if the parameter is present on the request. The name
		///     of the query parameter is inferred to be a
		///     camelCased version of the C# property name.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByQueryOpt<T>(
			Expression<Func<TModel, T>> property,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByQueryOpt(property, SeltzrOptionsBuilderBase.CamelCase(property), comparer);

		/// <summary>
		///     Filters this route's dataset by a query parameter value only if the parameter is present on the request
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="queryParameterName">
		///     The name of the query parameter whose value to compare with the value of
		///     <paramref name="property" /> on the dataset's <typeparamref name="TModel" /> objects
		/// </param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByQueryOpt<T>(
			Expression<Func<TModel, T>> property,
			string queryParameterName,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByParameterOpt(property, new QueryParameterRetriever(queryParameterName), comparer);

		/// <summary>
		///     Filters this route's dataset by a route value.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="routeValueName">The name of the route value to filter by</param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByRoute<T>(
			Expression<Func<TModel, T>> property,
			string routeValueName,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByParameter(property, new RouteValueRetriever(routeValueName), comparer);

		/// <summary>
		///     Filters this route's dataset by a route value. The name of the route value is inferred to be a camelCased version
		///     of the C# property name.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByRoute<T>(
			Expression<Func<TModel, T>> property,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByRoute(property, SeltzrOptionsBuilderBase.CamelCase(property), comparer);

		/// <summary>
		///     Filters this route's dataset by a route parameter value. The name of the route value is inferred to be a camelCased
		///     version of the C# property name.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByRouteEqual<T>(Expression<Func<TModel, T>> property) =>
			this.FilterByRouteEqual(property, SeltzrOptionsBuilderBase.CamelCase(property));

		/// <summary>
		///     Filters this route's dataset by a route parameter value
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="routeValueName">
		///     The name of the route parameter whose value to compare with the value of
		///     <paramref name="property" /> on the dataset's <typeparamref name="TModel" /> objects
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByRouteEqual<T>(
			Expression<Func<TModel, T>> property,
			string routeValueName) =>
			this.FilterByParameterEqual(property, new RouteValueRetriever(routeValueName));

		/// <summary>
		///     Filters this route's dataset by a route parameter value only if the parameter is present on the request. The name
		///     of the route value is inferred to be a camelCased
		///     version of the C# property name.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByRouteEqualOpt<T>(Expression<Func<TModel, T>> property) =>
			this.FilterByRouteEqualOpt(property, SeltzrOptionsBuilderBase.CamelCase(property));

		/// <summary>
		///     Filters this route's dataset by a route parameter value only if the parameter is present on the request
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="routeValueName">
		///     The name of the route parameter whose value to compare with the value of
		///     <paramref name="property" /> on the dataset's <typeparamref name="TModel" /> objects
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByRouteEqualOpt<T>(
			Expression<Func<TModel, T>> property,
			string routeValueName) =>
			this.FilterByParameterEqualOpt(property, new RouteValueRetriever(routeValueName));

		/// <summary>
		///     Filters this route's dataset by a route value only if the parameter is present on the request.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="routeValueName">The name of the route value to filter by</param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByRouteOpt<T>(
			Expression<Func<TModel, T>> property,
			string routeValueName,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByParameterOpt(property, new RouteValueRetriever(routeValueName), comparer);

		/// <summary>
		///     Filters this route's dataset by a route value only if the parameter is present on the request. The name of the
		///     route value is inferred to be a camelCased version
		///     of the C# property name.
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterByRouteOpt<T>(
			Expression<Func<TModel, T>> property,
			Expression<Func<T, T, bool>> comparer) =>
			this.FilterByRouteOpt(property, SeltzrOptionsBuilderBase.CamelCase(property), comparer);

		/// <summary>
		///     Filters this route's dataset only including the elements that match the predicate
		/// </summary>
		/// <param name="predicate">The predicate to use to determine if an element should be included in the dataset or not</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterWhere(Expression<Func<TModel, bool>> predicate) {
			this.Filter(d => d.Where(predicate));
			return this;
		}

		/// <summary>
		///     Filters this route's dataset only including the elements that match the predicate
		/// </summary>
		/// <param name="predicate">The predicate to use to determine if an element should be included in the dataset or not</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> FilterWhere(
			Expression<Func<IApiContext<TModel, TUser>, TModel, bool>> predicate) {
			this.Filter((c, d) => d.Where(new FilterWhereExpressionModifier<TModel, TUser>(c).Modify(predicate)));
			return this;
		}

		/// <summary>
		///     Limits this route's dataset to only the first element, throwing if there are no elements in the set
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> First() {
			this.Limit(1);
			this.RequireNonEmpty();
			return this;
		}

		/// <summary>
		///     Limits this route's dataset to a certain number of elements
		/// </summary>
		/// <param name="count">The number of elements to limit the result to</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Limit(int count) => this.Filter(d => d.Take(count));

		/// <summary>
		///     Limits this route's dataset to only one element
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> LimitOne() => this.Limit(1);

		/// <summary>
		///     Limits this route's dataset to a certain number of elements, as determined by a query parameter
		/// </summary>
		/// <param name="paramName">The name of the query parameter that determines the number of elements in the dataset</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> LimitQuery(string paramName) {
			this.Filter(
				(c, d) => {
					if (!c.Request.Query.ContainsKey(paramName)) return d;
					int Limit = int.Parse(c.Request.Query[paramName]);
					if (Limit < 0)
						throw new ArgumentOutOfRangeException(paramName, Limit, "Limit must be non-negative");
					return d.Take(Limit);
				});
			return this;
		}

		/// <summary>
		///     Limits this route's dataset to a certain number of elements, as determined by a query parameter
		/// </summary>
		/// <param name="paramName">The name of the query parameter that determines the number of elements in the dataset</param>
		/// <param name="max">The maximum number of elements to return</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> LimitQuery(string paramName, int max) {
			this.LimitQuery(paramName);
			this.Limit(max);
			return this;
		}

		/// <summary>
		///     Orders this route's dataset in an ascending order using the given key selector
		/// </summary>
		/// <param name="predicate">The predicate to use to give they key to sort by</param>
		/// <typeparam name="TKey">The type of the key to sort by</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> OrderBy<TKey>(Expression<Func<TModel, TKey>> predicate) {
			this.Filter(d => d.OrderBy(predicate));
			return this;
		}

		/// <summary>
		///     Orders this route's dataset in an ascending order using the given key selector
		/// </summary>
		/// <param name="predicate">The predicate to use to give they key to sort by</param>
		/// <param name="comparer">The comparer to use to compare two elements in the dataset</param>
		/// <typeparam name="TKey">The type of the key to sort by</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> OrderBy<TKey>(
			Expression<Func<TModel, TKey>> predicate,
			IComparer<TKey> comparer) {
			this.Filter(d => d.OrderBy(predicate, comparer));
			return this;
		}

		/// <summary>
		///     Orders this route's dataset in an descending order using the given key selector
		/// </summary>
		/// <param name="predicate">The predicate to use to give they key to sort by</param>
		/// <typeparam name="TKey">The type of the key to sort by</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> OrderByDescending<TKey>(Expression<Func<TModel, TKey>> predicate) {
			this.Filter(d => d.OrderByDescending(predicate));
			return this;
		}

		/// <summary>
		///     Orders this route's dataset in an descending order using the given key selector
		/// </summary>
		/// <param name="predicate">The predicate to use to give they key to sort by</param>
		/// <param name="comparer">The comparer to use to compare two elements in the dataset</param>
		/// <typeparam name="TKey">The type of the key to sort by</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> OrderByDescending<TKey>(
			Expression<Func<TModel, TKey>> predicate,
			IComparer<TKey> comparer) {
			this.Filter(d => d.OrderByDescending(predicate, comparer));
			return this;
		}

		/// <summary>
		///     Enables one-indexed pagination of the API output using the "page" and "count" query parameters with infinite
		///     maximum page sizes
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Paginate() => this.Paginate("page", "count");

		/// <summary>
		///     Enables one-indexed pagination of the API output with infinite maximum page sizes
		/// </summary>
		/// <param name="pageParamName">The name of the parameter that defines what page to return, starting from zero</param>
		/// <param name="countParamName">The name of the parameter that defines how many elements to return</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Paginate(string pageParamName, string countParamName) =>
			this.Paginate(pageParamName, countParamName, 0);

		/// <summary>
		///     Enables one-indexed pagination of the API output
		/// </summary>
		/// <param name="pageParamName">The name of the parameter that defines what page to return, starting from zero</param>
		/// <param name="countParamName">The name of the parameter that defines how many elements to return</param>
		/// <param name="maxPageSize">The maximum number of elements to return in a page</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Paginate(
			string pageParamName,
			string? countParamName,
			int maxPageSize) {
			if (maxPageSize < 0)
				throw new ArgumentOutOfRangeException(
					nameof(maxPageSize),
					maxPageSize,
					"Max page size must be non-negative. Use overload without parameter to allow infinite page sizes.");
			this.Filter(
				(c, d) => {
					int Page = 1;
					int PageSize = 0;
					int TotalElements = d.Count();
					if (c.Request.Query.ContainsKey(pageParamName))
						Page = int.Parse(c.Request.Query[pageParamName]);
					if (countParamName != null && c.Request.Query.ContainsKey(countParamName))
						PageSize = int.Parse(c.Request.Query[countParamName]);
					if (Page < 1)
						throw new ArgumentOutOfRangeException(pageParamName, Page, "Page must be at least one");
					if (PageSize < 0)
						throw new ArgumentOutOfRangeException(countParamName, PageSize, "Count must be non-negative");
					if (maxPageSize != 0 && (PageSize > maxPageSize || PageSize == 0)) PageSize = maxPageSize;
					if (maxPageSize == 0 && PageSize == 0)
						return d;

					c.Response?.SetString<CurrentPageAttribute>(Page.ToString());
					c.Response?.SetString<TotalPagesAttribute>(
						Math.Ceiling((double)TotalElements / PageSize).ToString("0"));
					return d.Skip((Page - 1) * PageSize).Take(PageSize);
				});
			return this;
		}

		/// <summary>
		///     Enables one-indexed pagination of the API output
		/// </summary>
		/// <param name="pageParamName">The name of the query parameter that defines what page to return</param>
		/// <param name="pageSize">The number of elements to include in each page</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Paginate(string pageParamName, int pageSize) =>
			this.Paginate(pageParamName, null, pageSize);

		/// <summary>
		///     Skips over a given number of elements in the dataset
		/// </summary>
		/// <param name="count">The number of elements in the dataset to skip over</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Skip(int count) => this.Filter(d => d.Skip(count));

		/// <summary>
		///     Skips a certain number of elements in this route's dataset, as determined by a query parameter
		/// </summary>
		/// <param name="paramName">The name of the query parameter that determines the number of elements in the dataset to skip</param>
		/// <param name="multiplier">
		///     The number to multiply the value of <paramref name="paramName" /> by before skipping, for
		///     pagination
		/// </param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> SkipQuery(string paramName, int multiplier = 1) {
			this.Filter(
				(c, d) => {
					if (!c.Request.Query.ContainsKey(paramName)) return d;
					int Skip = int.Parse(c.Request.Query[paramName]);
					if (Skip < 0)
						throw new ArgumentOutOfRangeException(paramName, Skip, "Skip value must be non-negative");
					return d.Skip(Skip * multiplier);
				});
			return this;
		}

		/// <summary>
		///     Filters this route's dataset by a parameter value
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="retriever">A function that will get the value of the parameter for a given request</param>
		/// <param name="comparer">
		///     A method which is passed the property value and parsed parameter value, and returns a boolean
		///     value indicating whether to include the model in the dataset or not
		/// </param>
		/// <param name="optional">
		///     <see langword="true" /> to only apply the filter if the request parameter is present,
		///     <see langword="false" /> to always throw if the parameter is not present
		/// </param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		private SeltzrOptionsBuilder<TModel, TUser> FilterByParameter<T>(
			Expression<Func<TModel, T>> property,
			ParameterRetriever retriever,
			Expression<Func<T, T, bool>> comparer,
			bool optional) {
			PropertyInfo Info = SeltzrOptionsBuilderBase.ExtractProperty(property);
			Type PropertyType = typeof(T);

			this.Filter(
				(c, d) => {
					string? ParamValue = retriever.GetValue(c.Request);
					if (ParamValue == null) 
						return optional ? d : throw new ConditionFailedException("Failed to parse request parameter");

					T Parsed = (T)ParameterResolver.ParseParameter(ParamValue, PropertyType);

					return d.Where(new FilterByExpressionModifier<TModel, T>(Info, Parsed).Modify(comparer));
				});
			return this;
		}
	}
}