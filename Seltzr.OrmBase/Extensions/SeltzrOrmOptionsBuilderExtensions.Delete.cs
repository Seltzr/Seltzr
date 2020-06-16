// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilderExtensions.Delete.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace - keep it all together
namespace Seltzr.Extensions {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.AspNetCore.Routing.Template;

	using Seltzr.Exceptions;
	using Seltzr.Operations;
	using Seltzr.Options.Builder;
	using Seltzr.OrmBase.Options;
	using Seltzr.ParameterRetrievers;

	/// <summary>
	///     Extension methods for the <see cref="SeltzrOptionsBuilder{TModel,TUser}" /> class
	/// </summary>
	public static partial class SeltzrOrmOptionsBuilderExtensions {
		/// <summary>
		///     Sets this route up to handle a DELETE request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>
		///     If not used with any filters, this route will drop all models in the dataset
		/// </remarks>
		public static SeltzrOptionsBuilder<TModel, TUser> Delete<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			if (!(builder is IOrmSeltzrOptionsBuilder<TModel, TUser> OrmBuilder))
				throw new OptionsException(
					$"Delete may only be called on a builder that inherits from {nameof(IOrmSeltzrOptionsBuilder<TModel, TUser>)}");
			IOperation<TModel, TUser> DeleteOperation = OrmBuilder.GetDeleteOperation();
			return builder.SetupDelete(
				routePattern,
				o => {
					o.UseOperation(DeleteOperation);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a DELETE by primary key request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameters">The parameters that will make up the primary key to filter by</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			ParameterRetriever[] parameters,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.Delete(
				routePattern,
				o => {
					o.FilterByPrimaryKey(parameters);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a DELETE request by route values using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterNames">The names of the route value parameters to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string[] parameterNames,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.Delete(
				routePattern,
				o => {
					o.FilterByPrimaryKeyRoute(parameterNames);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a DELETE request by route values using an ORM. The names of the route values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			return builder.DeleteByPrimaryKey(null);
		}

		/// <summary>
		///     Sets this route up to handle a DELETE request by route values using an ORM. The names of the route values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class {
			string UpdatedPattern = string.Empty;
			IEnumerable<string> ExistingNames =
				TemplateParser.Parse(builder.RoutePattern).Parameters.Select(p => p.Name);
			string[] KeyNames = SeltzrOrmOptionsBuilderExtensions.GetKeyNames(builder);

			foreach (string KeyName in KeyNames.Except(ExistingNames)) {
				if (!UpdatedPattern.EndsWith("/")) UpdatedPattern += "/";
				UpdatedPattern += $"{{{KeyName}}}/";
			}

			return builder.DeleteByPrimaryKey(UpdatedPattern, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a DELETE request by route values using an ORM. The names of the route values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			string UpdatedPattern = routePattern;
			string[] ExistingNames = TemplateParser.Parse(routePattern).Parameters.Select(p => p.Name).ToArray();
			string[] KeyNames = SeltzrOrmOptionsBuilderExtensions.GetKeyNames(builder);

			foreach (string KeyName in KeyNames.Except(ExistingNames)) {
				if (!UpdatedPattern.EndsWith("/")) UpdatedPattern += "/";
				UpdatedPattern += $"{{{KeyName}}}/";
			}

			return builder.DeleteByPrimaryKey(UpdatedPattern, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a DELETE request by route values using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterName">The name of the route value parameter to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string parameterName,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.DeleteByPrimaryKey(routePattern, new[] { parameterName }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a DELETE request by query parameters using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterNames">The names of the route value parameters to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKeyQuery<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string[] parameterNames,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.SetupDelete(
				routePattern,
				o => {
					o.FilterByPrimaryKeyQuery(parameterNames);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a DELETE request by a primary key query parameter using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterName">The name of the route value parameter to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKeyQuery<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string parameterName,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.DeleteByPrimaryKeyQuery(routePattern, new[] { parameterName }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a DELETE request by query parameters using an ORM. The names of the query
		///     parameters will be camelCased names of the primary key of the entity
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKeyQuery<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.DeleteByPrimaryKeyQuery(
				routePattern,
				SeltzrOrmOptionsBuilderExtensions.GetKeyNames(builder),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a DELETE request by query parameters on the same route pattern using an ORM.
		///     The names of the query parameters will be camelCased names of the primary key of the entity
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> DeleteByPrimaryKeyQuery<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.DeleteByPrimaryKeyQuery("", SeltzrOrmOptionsBuilderExtensions.GetKeyNames(builder), optionsHandler);
	}
}