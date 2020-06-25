// -----------------------------------------------------------------------
// <copyright file="EntityFrameworkSeltzrOptionsBuilder.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.EntityFramework.Options {
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Data.Entity.Core.Objects;
	using System.Data.Entity.Infrastructure;
	using System.Linq;
	using System.Reflection;

	using Microsoft.AspNetCore.Builder;
	using Microsoft.Extensions.DependencyInjection;

	using Seltzr.EntityFramework.Operations;
	using Seltzr.Exceptions;
	using Seltzr.Operations;
	using Seltzr.Options;
	using Seltzr.Options.Builder;
	using Seltzr.OrmBase;
	using Seltzr.OrmBase.Options;
	using Seltzr.ParameterRetrievers;

	/// <summary>Options builder for an Entity Framework based API</summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public class EntityFrameworkSeltzrOptionsBuilder<TModel, TUser> : SeltzrOptionsBuilder<TModel, TUser>,
	                                                                  IOrmSeltzrOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The primary key for <typeparamref name="TModel" />
		/// </summary>
		private List<KeyProperty>? PrimaryKey;

		/// <summary>
		///     Initializes a new instance of the <see cref="EntityFrameworkSeltzrOptionsBuilder{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="app">The application builder for the app</param>
		/// <param name="contextType">
		///     The type of the database context to get the set of <typeparamref name="TModel" /> objects
		///     from
		/// </param>
		/// <param name="primaryKey">
		///     The primary key for <typeparamref name="TModel" />. If null, it will be determined
		///     automatically
		/// </param>
		/// <param name="baseRoute">The base route for these options</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		internal EntityFrameworkSeltzrOptionsBuilder(
			IApplicationBuilder app,
			Type contextType,
			List<PropertyInfo>? primaryKey,
			string baseRoute,
			Action<IEndpointConventionBuilder>? routeOptionsHandler)
			: base(baseRoute, routeOptionsHandler) {
			this.ContextType = contextType;
			this.App = app;
			this.PrimaryKey = primaryKey?.Select(p => new KeyProperty(p)).ToList();
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="EntityFrameworkSeltzrOptionsBuilder{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="app">The application builder for the app</param>
		/// <param name="contextType">
		///     The type of the database context to get the set of <typeparamref name="TModel" /> objects
		///     from
		/// </param>
		/// <param name="primaryKey">
		///     The primary key for <typeparamref name="TModel" />. If null, it will be determined
		///     automatically
		/// </param>
		/// <param name="options">The existing options</param>
		internal EntityFrameworkSeltzrOptionsBuilder(
			IApplicationBuilder app,
			Type contextType,
			List<KeyProperty>? primaryKey,
			SeltzrOptions<TModel, TUser>? options)
			: base(options) {
			this.ContextType = contextType;
			this.App = app;
			this.PrimaryKey = primaryKey;
		}

		/// <summary>
		///     Gets the application builder for this application
		/// </summary>
		internal IApplicationBuilder App { get; }

		/// <summary>
		///     Gets the type of the database context to get the set of <typeparamref name="TModel" /> objects from
		/// </summary>
		internal Type ContextType { get; }

		/// <summary>
		///     Creates a child instance of the <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> type sharing the given base
		///     options. When overriden in a derived class, this method can be used to ensure that the entire tree of
		///     <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> objects share the same derived type
		/// </summary>
		/// <param name="baseOptions">The base options for the new instance</param>
		/// <returns>The new <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> instance</returns>
		public override SeltzrOptionsBuilder<TModel, TUser> CreateChild(SeltzrOptions<TModel, TUser>? baseOptions) =>
			new EntityFrameworkSeltzrOptionsBuilder<TModel, TUser>(
				this.App,
				this.ContextType,
				this.PrimaryKey,
				baseOptions);

		/// <summary>
		///     Gets a create <see cref="IOperation{TModel, TUser}" /> for this model
		/// </summary>
		/// <returns>A new create operation for <typeparamref name="TModel" /></returns>
		public virtual IOperation<TModel, TUser> GetCreateOperation() => this.NewOperation(typeof(CreateOperation<,>));

		/// <summary>
		///     Gets a delete <see cref="IOperation{TModel, TUser}" /> for this model
		/// </summary>
		/// <returns>A new delete operation for <typeparamref name="TModel" /></returns>
		public virtual IOperation<TModel, TUser> GetDeleteOperation() => this.NewOperation(typeof(DeleteOperation<,>));

		/// <summary>
		///     Gets the properties that make up the primary key of <typeparamref name="TModel" />
		/// </summary>
		/// <returns>The properties that make up the primary key of <typeparamref name="TModel" /></returns>
		public virtual List<KeyProperty> GetPrimaryKey() {
			if (this.PrimaryKey != null) return this.PrimaryKey;

			using IServiceScope Scope = this.App.ApplicationServices.CreateScope();
			DbContext Context = (DbContext)Scope.ServiceProvider.GetService(this.ContextType);
			ObjectContext ObjectContext = ((IObjectContextAdapter)Context).ObjectContext;
			ObjectSet<TModel> Set = ObjectContext.CreateObjectSet<TModel>();

			PropertyInfo[] ModelProperties = typeof(TModel).GetProperties();

			try {
				this.PrimaryKey = Set.EntitySet.ElementType.KeyMembers
					.Select(k => new KeyProperty(ModelProperties.First(p => p.Name == k.Name))).ToList();
				return this.PrimaryKey;
			}
			catch (InvalidOperationException) {
				throw new OptionsException(
					$"Unable to determine primary key for ${typeof(TModel).Name} automatically. Try specifying it explicitly");
			}
		}

		/// <summary>
		///     Gets an update <see cref="IOperation{TModel, TUser}" /> for this model
		/// </summary>
		/// <param name="properties">
		///     The properties to use to compare existing models with parsed models to determine which models
		///     to update
		/// </param>
		/// <param name="retrievers">
		///     A list of parameter retrievers to use to get values for the properties. If null, the parsed
		///     body will be used instead
		/// </param>
		/// <returns>A new update operation for <typeparamref name="TModel" /></returns>
		public virtual IOperation<TModel, TUser> GetUpdateOperation(
			PropertyInfo[] properties,
			ParameterRetriever[]? retrievers) {
			return (IOperation<TModel, TUser>)Activator.CreateInstance(
				typeof(UpdateOperation<,>).MakeGenericType(typeof(TModel), this.ContextType),
				new object?[] { properties, retrievers })!;
		}

		/// <summary>
		///     Creates a new operation
		/// </summary>
		/// <param name="operationType">The type of operation to create</param>
		/// <returns>The created operation</returns>
		private IOperation<TModel, TUser> NewOperation(Type operationType) {
			IOperation<TModel, TUser> Operation =
				(IOperation<TModel, TUser>)Activator.CreateInstance(
					operationType.MakeGenericType(typeof(TModel), this.ContextType))!;
			return Operation;
		}
	}
}