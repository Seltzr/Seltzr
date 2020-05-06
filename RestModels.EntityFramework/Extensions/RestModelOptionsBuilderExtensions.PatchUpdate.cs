﻿// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilderExtensions.PatchUpdate.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Extensions {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	using Microsoft.AspNetCore.Routing.Template;

	using RestModels.Options.Builder;
	using RestModels.ParameterRetrievers;

	/// <summary>
	///     Extension methods for the <see cref="RestModelOptionsBuilder{TModel,TUser}" /> class
	/// </summary>
	public static partial class RestModelOptionsBuilderExtensions {
		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     route value to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="routeValues">The route value names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			string[] routeValues,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByParam(
				routePattern,
				propertyExpressions,
				routeValues.Select(p => (ParameterRetriever)new RouteValueRetriever(p)).ToArray(),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     route value to determine which elements to update. The names of the route values will be camelCase versions of the
		///     C# properties to compare. If these route values do not yet exist, they will be added to the end of the route
		///     pattern in order.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class {
			string ExistingPattern = $"{builder.RoutePattern}{(builder.RoutePattern.EndsWith("/") ? "" : "/")}";
			if (routePattern.StartsWith("/")) ExistingPattern += routePattern.Substring(1);
			else ExistingPattern += routePattern;

			string NewPattern = routePattern;
			IEnumerable<string> ExistingNames = TemplateParser.Parse(ExistingPattern).Parameters.Select(p => p.Name);
			string[] KeyNames = propertyExpressions.Select(RestModelOptionsBuilderExtensions.ExtractProperty)
				.Select(p => RestModelOptionsBuilderExtensions.CamelCase(p.Name)).ToArray();
			foreach (string KeyName in KeyNames.Except(ExistingNames)) {
				if (!NewPattern.EndsWith("/")) NewPattern += "/";
				NewPattern += $"{{{KeyName}}}/";
			}

			return builder.PatchUpdateBy(NewPattern, propertyExpressions, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework at the same route pattern, comparing the
		///     given property with a route value to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="routeValues">The route value names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			string[] routeValues,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateBy("", propertyExpressions, routeValues, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     route value to determine which elements to update. The names of the route values will be camelCase versions of the
		///     C# properties to compare. If these route values do not yet exist, they will be added to the end of the route
		///     pattern in order.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PatchUpdateBy("", propertyExpressions, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     route value to determine which elements to update. The names of the route values will be camelCase versions of the
		///     C# properties to compare. If these route values do not yet exist, they will be added to the end of the route
		///     pattern in order.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PatchUpdateBy(propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     route value to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="routeValue">The route value names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			string routeValue,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateBy(routePattern, new[] { propertyExpression }, new[] { routeValue }, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     route value to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="routeValue">The route value names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			string routeValue,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateBy("", propertyExpression, routeValue, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework at the same route, comparing the given
		///     property with a route value to determine which elements to update. The name of the route value will be a camelCase
		///     version of the C# property to compare. If this route value does not yet exist, it will be added to the end of the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateBy(new[] { propertyExpression }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework at the same route, comparing the given
		///     property with a route value to determine which elements to update. The name of the route value will be a camelCase
		///     version of the C# property to compare. If this route value does not yet exist, it will be added to the end of the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateBy(routePattern, new[] { propertyExpression }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework at the same route, comparing the given
		///     property with a route value to determine which elements to update. The name of the route value will be a camelCase
		///     version of the C# property to compare. If this route value does not yet exist, it will be added to the end of the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PatchUpdateBy(routePattern, propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework. The property to compare is assumed to be
		///     directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateByParam(routePattern, new[] { propertyExpression }, null, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework. The property to compare is assumed to be
		///     directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByParam("", propertyExpression, null, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework. The properties to compare are assumed to
		///     be directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByBody(routePattern, propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework. The properties to compare are assumed to
		///     be directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByParam(routePattern, propertyExpressions, null, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework. The properties to compare are assumed to
		///     be directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByParam(propertyExpressions, null, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework. The properties to compare are assumed to
		///     be directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByParam(propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="retrievers">The parameters to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByParam<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			ParameterRetriever[]? retrievers,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			PropertyInfo[] Infos = propertyExpressions.Select(RestModelOptionsBuilderExtensions.ExtractProperty)
				.ToArray();
			return builder.SetupPatch(
				routePattern,
				o => {
					o.UseOperation(RestModelOptionsBuilderExtensions.CreateUpdateOperation(builder, Infos, retrievers));
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="retriever">The parameter to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByParam<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			ParameterRetriever? retriever,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateByParam(
				routePattern,
				new[] { propertyExpression },
				retriever == null ? null : new[] { retriever },
				optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <param name="retrievers">The parameters to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByParam<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			ParameterRetriever[]? retrievers,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByParam("", propertyExpressions, retrievers, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update by primary key request using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameters">The parameters that will make up the primary key to filter by</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			ParameterRetriever[] parameters,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			PropertyInfo[] Properties = RestModelOptionsBuilderExtensions.GetKeyProperties(builder)
				.Select(p => p.PropertyInfo).ToArray();

			return builder.SetupPatch(
				routePattern,
				o => {
					o.UseOperation(
						RestModelOptionsBuilderExtensions.CreateUpdateOperation(builder, Properties, parameters));
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request by route values using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterNames">The names of the route value parameters to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string[] parameterNames,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateByPrimaryKey(
				routePattern,
				parameterNames.Select(p => (ParameterRetriever)new RouteValueRetriever(p)).ToArray(),
				optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request by route values using Entity Framework. The names of the route
		///     values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByPrimaryKey(null);

		/// <summary>
		///     Sets this route up to handle a PATCH update request by route values using Entity Framework. The names of the route
		///     values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByPrimaryKey("", optionsHandler);

		/// <summary>
		///     Sets a route up to handle a PATCH update request by route values using Entity Framework. The names of the route
		///     values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			string ExistingPattern = $"{builder.RoutePattern}{(builder.RoutePattern.EndsWith("/") ? "" : "/")}";
			if (routePattern.StartsWith("/")) ExistingPattern += routePattern.Substring(1);
			else ExistingPattern += routePattern;

			string NewPattern = routePattern;
			IEnumerable<string> ExistingNames = TemplateParser.Parse(ExistingPattern).Parameters.Select(p => p.Name);
			string[] KeyNames = RestModelOptionsBuilderExtensions.GetKeyNames(builder);
			foreach (string KeyName in KeyNames.Except(ExistingNames)) {
				if (!NewPattern.EndsWith("/")) NewPattern += "/";
				NewPattern += $"{{{KeyName}}}/";
			}

			return builder.PatchUpdateByPrimaryKey(NewPattern, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets a route up to handle a PATCH update request by route values using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterName">The name of the route value parameter to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string parameterName,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateByPrimaryKey(routePattern, new[] { parameterName }, optionsHandler);
		}

		/// <summary>
		///     Sets a route up to handle a PATCH update request by query parameters using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterNames">The names of the route value parameters to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string[] parameterNames,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateByPrimaryKey(
				routePattern,
				parameterNames.Select(p => (ParameterRetriever)new QueryParameterRetriever(p)).ToArray(),
				optionsHandler);
		}

		/// <summary>
		///     Sets a route up to handle a PATCH update request by a primary key query parameter using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterName">The name of the route value parameter to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string parameterName,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateByPrimaryKeyQuery(routePattern, new[] { parameterName }, optionsHandler);
		}

		/// <summary>
		///     Sets a route up to handle a PATCH update request by query parameters using Entity Framework. The names of the
		///     query
		///     parameters will be camelCased names of the primary key of the entity
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByPrimaryKeyQuery(
				routePattern,
				RestModelOptionsBuilderExtensions.GetKeyNames(builder),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request by query parameters on the same route pattern using Entity
		///     Framework.
		///     The names of the query parameters will be camelCased names of the primary key of the entity
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByPrimaryKeyQuery(
				"",
				RestModelOptionsBuilderExtensions.GetKeyNames(builder),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     query parameter to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="queryParameters">The query parameter names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			string[] queryParameters,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByParam(
				routePattern,
				propertyExpressions,
				queryParameters.Select(p => (ParameterRetriever)new QueryParameterRetriever(p)).ToArray(),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     query parameter to determine which elements to update. The names of the query parameters will be camelCase versions
		///     of the
		///     C# properties to compare.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class {
			string[] KeyNames = propertyExpressions.Select(RestModelOptionsBuilderExtensions.ExtractProperty)
				.Select(p => RestModelOptionsBuilderExtensions.CamelCase(p.Name)).ToArray();
			return builder.PatchUpdateByQuery(routePattern, propertyExpressions, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework at the same route pattern, comparing the
		///     given property with a query parameter to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="queryParameters">The query parameter names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			string[] queryParameters,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByQuery("", propertyExpressions, queryParameters, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     query parameter to determine which elements to update. The names of the query parameters will be camelCase versions
		///     of the
		///     C# properties to compare.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByQuery("", propertyExpressions, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     query parameter to determine which elements to update. The names of the query parameters will be camelCase versions
		///     of the
		///     C# properties to compare.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByQuery(propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     query parameter to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="queryParameter">The query parameter names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			string queryParameter,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByQuery(
				routePattern,
				new[] { propertyExpression },
				new[] { queryParameter },
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework, comparing the given property with a
		///     query parameter to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="queryParameter">The query parameter names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			string queryParameter,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByQuery("", propertyExpression, queryParameter, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework at the same route, comparing the given
		///     property with a query parameter to determine which elements to update. The name of the query parameter will be a
		///     camelCase
		///     version of the C# property to compare. If this query parameter does not yet exist, it will be added to the end of
		///     the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateByQuery(new[] { propertyExpression }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework at the same route, comparing the given
		///     property with a query parameter to determine which elements to update. The name of the query parameter will be a
		///     camelCase
		///     version of the C# property to compare. If this query parameter does not yet exist, it will be added to the end of
		///     the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PatchUpdateByQuery(routePattern, new[] { propertyExpression }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PATCH update request using Entity Framework at the same route, comparing the given
		///     property with a query parameter to determine which elements to update. The name of the query parameter will be a
		///     camelCase
		///     version of the C# property to compare. If this query parameter does not yet exist, it will be added to the end of
		///     the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PatchUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PatchUpdateByQuery(routePattern, propertyExpressions, null);
	}
}