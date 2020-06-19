// -----------------------------------------------------------------------
// <copyright file="EntityFrameworkSeltzrApplicationBuilderExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace -- recommended by MS

namespace Microsoft.Extensions.DependencyInjection {
	using System;
	using System.Linq;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.ChangeTracking;

	using Seltzr.Auth;
	using Seltzr.EntityFrameworkCore;
	using Seltzr.EntityFrameworkCore.Options;
	using Seltzr.Options.Builder;

	/// <summary>
	///     Extension methods for the <see cref="IApplicationBuilder" /> interface.
	/// </summary>
	public static class EFCoreSeltzrApplicationBuilderExtensions {
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
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder AddEFCoreSeltzr<TModel, TContext, TUser>(
			this IApplicationBuilder app,
			string route,
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler,
			Action<IEndpointConventionBuilder>? routeOptionsHandler)
			where TModel : class where TContext : DbContext where TUser : class {
			SeltzrOptionsBuilder<TModel, TUser> OptionsBuilder =
				new EntityFrameworkSeltzrOptionsBuilder<TModel, TUser>(app, typeof(TContext), route, routeOptionsHandler);

			OptionsBuilder.UseModelProvider(new EntityFrameworkModelProvider<TModel, TContext>());

			// disable change tracking post request
			OptionsBuilder.After((c, d) => {
				TContext Context = c.HttpContext.RequestServices.GetRequiredService<TContext>();
				foreach (EntityEntry<TModel> Model in d.Select(Context.Entry))
					Model.State = EntityState.Detached;
			});
			optionsHandler(OptionsBuilder);

			return app.AddSeltzr(OptionsBuilder.BuildAll());
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
		public static IApplicationBuilder AddEFCoreSeltzr<TModel, TContext, TUser>(
			this IApplicationBuilder app,
			string route,
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TContext : DbContext where TUser : class {
			return app.AddEFCoreSeltzr<TModel, TContext, TUser>(route, optionsHandler, null);
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
		public static IApplicationBuilder AddEFCoreSeltzr<TModel, TContext, TUser>(
			this IApplicationBuilder app,
			Action<SeltzrOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TContext : DbContext where TUser : class {
			return app.AddEFCoreSeltzr<TModel, TContext, TUser>("/", optionsHandler, null);
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
		public static IApplicationBuilder AddEFCoreSeltzr<TModel, TContext>(
			this IApplicationBuilder app,
			string route,
			Action<SeltzrOptionsBuilder<TModel, NoUser>> optionsHandler,
			Action<IEndpointConventionBuilder>? routeOptionsHandler)
			where TModel : class where TContext : DbContext {
			return app.AddEFCoreSeltzr<TModel, TContext, NoUser>(
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
		public static IApplicationBuilder AddEFCoreSeltzr<TModel, TContext>(
			this IApplicationBuilder app,
			string route,
			Action<SeltzrOptionsBuilder<TModel, NoUser>> optionsHandler)
			where TModel : class where TContext : DbContext {
			return app.AddEFCoreSeltzr<TModel, TContext>(route, optionsHandler, null);
		}

		/// <summary>
		///     Adds Seltzr middleware with Entity Framework to the app at the root ("/") endpoint without a user context
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <param name="app">The app to add Seltzr to</param>
		/// <param name="optionsHandler">A handler to set options for this Seltzr API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder AddEFCoreSeltzr<TModel, TContext>(
			this IApplicationBuilder app,
			Action<SeltzrOptionsBuilder<TModel, NoUser>> optionsHandler)
			where TModel : class where TContext : DbContext {
			return app.AddEFCoreSeltzr<TModel, TContext>("/", optionsHandler, null);
		}
	}
}