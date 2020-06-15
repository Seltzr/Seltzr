// -----------------------------------------------------------------------
// <copyright file="EntityFrameworkSeltzrApplicationBuilderExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace -- recommended by MS

namespace Microsoft.Extensions.DependencyInjection {
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Microsoft.AspNetCore.Builder;

	using Seltzr.Auth;
	using Seltzr.EntityFramework;
	using Seltzr.EntityFramework.Options;
	using Seltzr.Options.Builder;

	/// <summary>
	///     Extension methods for the <see cref="IApplicationBuilder" /> interface.
	/// </summary>
	public static class EntityFrameworkSeltzrApplicationBuilderExtensions {
		/// <summary>
		///     Adds Seltzr middleware with Entity Framework to the app
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add Seltzr to</param>
		/// <param name="route">The base route for Seltzr</param>
		/// <param name="optionsHandler">A handler to set options for this Seltzr API</param>
		/// <param name="routeOptionsHandler">A handler to set ASP.NET Core options</param>
		/// <param name="primaryKeyProperties">An array of properties that make up the primary key. If omitted, the primary key will be determined automatically</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseEntityFrameworkSeltzr<TModel, TContext, TUser>(
			this IApplicationBuilder app,
			string route,
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler,
			Action<IEndpointConventionBuilder>? routeOptionsHandler,
			params Expression<Func<TModel, object>>[] primaryKeyProperties)
			where TModel : class where TContext : DbContext where TUser : class {
			List<PropertyInfo>? PrimaryKey = primaryKeyProperties.Length == 0
				                                 ? null
				                                 : primaryKeyProperties.Select(
						                                 EntityFrameworkSeltzrApplicationBuilderExtensions
							                                 .ExtractProperty)
					                                 .ToList();
			SeltzrOptionsBuilder<TModel, TUser> OptionsBuilder =
				new EntityFrameworkSeltzrOptionsBuilder<TModel, TUser>(app, typeof(TContext), PrimaryKey, route, routeOptionsHandler);

			OptionsBuilder.UseModelProvider(new EntityFrameworkModelProvider<TModel, TContext>());
			optionsHandler(OptionsBuilder);

			return app.UseSeltzr(OptionsBuilder.BuildAll());
		}

		/// <summary>
		///     Adds Seltzr middleware with Entity Framework to the app
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add Seltzr to</param>
		/// <param name="route">The base route for Seltzr</param>
		/// <param name="optionsHandler">A handler to set options for this Seltzr API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseEntityFrameworkSeltzr<TModel, TContext, TUser>(
			this IApplicationBuilder app,
			string route,
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TContext : DbContext where TUser : class {
			return app.UseEntityFrameworkSeltzr<TModel, TContext, TUser>(route, optionsHandler, null);
		}

		/// <summary>
		///     Adds Seltzr middleware with Entity Framework to the app at the root ("/") endpoint
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add Seltzr to</param>
		/// <param name="optionsHandler">A handler to set options for this Seltzr API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseEntityFrameworkSeltzr<TModel, TContext, TUser>(
			this IApplicationBuilder app,
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TContext : DbContext where TUser : class {
			return app.UseEntityFrameworkSeltzr<TModel, TContext, TUser>("/", optionsHandler, null);
		}

		/// <summary>
		///     Adds Seltzr middleware with Entity Framework to the app without a user context
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <param name="app">The app to add Seltzr to</param>
		/// <param name="route">The base route for Seltzr</param>
		/// <param name="optionsHandler">A handler to set options for this Seltzr API</param>
		/// <param name="routeOptionsHandler">A handler to set ASP.NET Core options</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseEntityFrameworkSeltzr<TModel, TContext>(
			this IApplicationBuilder app,
			string route,
			Action<SeltzrOptionsBuilder<TModel, NoUser>> optionsHandler,
			Action<IEndpointConventionBuilder>? routeOptionsHandler)
			where TModel : class where TContext : DbContext {
			return app.UseEntityFrameworkSeltzr<TModel, TContext, NoUser>(
				route,
				optionsHandler,
				routeOptionsHandler);
		}

		/// <summary>
		///     Adds Seltzr middleware with Entity Framework to the app without a user context
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <param name="app">The app to add Seltzr to</param>
		/// <param name="route">The base route for Seltzr</param>
		/// <param name="optionsHandler">A handler to set options for this Seltzr API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseEntityFrameworkSeltzr<TModel, TContext>(
			this IApplicationBuilder app,
			string route,
			Action<SeltzrOptionsBuilder<TModel, NoUser>> optionsHandler)
			where TModel : class where TContext : DbContext {
			return app.UseEntityFrameworkSeltzr<TModel, TContext>(route, optionsHandler, null);
		}

		/// <summary>
		///     Adds Seltzr middleware with Entity Framework to the app at the root ("/") endpoint without a user context
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <param name="app">The app to add Seltzr to</param>
		/// <param name="optionsHandler">A handler to set options for this Seltzr API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseEntityFrameworkSeltzr<TModel, TContext>(
			this IApplicationBuilder app,
			Action<SeltzrOptionsBuilder<TModel, NoUser>> optionsHandler)
			where TModel : class where TContext : DbContext {
			return app.UseEntityFrameworkSeltzr<TModel, TContext>("/", optionsHandler, null);
		}

		/// <summary>
		///     Extracts a property from an expression
		/// </summary>
		/// <param name="propertyExpression">The expression that refers to a property</param>
		/// <typeparam name="TModel">The declaring type of the property to extract</typeparam>
		/// <typeparam name="TProperty">The type of the property to extract</typeparam>
		/// <returns>The extracted property</returns>
		private static PropertyInfo ExtractProperty<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression) {
			MemberExpression MemberExpression;
			if (propertyExpression.Body is UnaryExpression Body)
				MemberExpression = (MemberExpression)Body.Operand;
			else MemberExpression = (MemberExpression)propertyExpression.Body;

			return (PropertyInfo)MemberExpression.Member;
		}
	}
}