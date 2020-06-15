// -----------------------------------------------------------------------
// <copyright file="EntityFrameworkSeltzrOptionsBuilder.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.OrmBase.Options {
	using System.Collections.Generic;
	using System.Reflection;

	using Seltzr.Operations;
	using Seltzr.OrmBase;
	using Seltzr.ParameterRetrievers;

	/// <summary>Contract for an options builder for an ORM-based API</summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public interface IOrmSeltzrOptionsBuilder<TModel, in TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Gets the properties that make up the primary key of <see cref="TModel" />
		/// </summary>
		/// <returns>The properties that make up the primary key of <see cref="TModel" /></returns>
		List<KeyProperty> GetPrimaryKey();

		/// <summary>
		///		Gets a create <see cref="IOperation{TModel, TUser}"/> for this model
		/// </summary>
		/// <returns>A new create operation for <see cref="TModel"/></returns>
		IOperation<TModel, TUser> GetCreateOperation();

		/// <summary>
		///		Gets an update <see cref="IOperation{TModel, TUser}"/> for this model
		/// </summary>
		/// <param name="properties">The properties to use to compare existing models with parsed models to determine which models to update</param>
		/// <param name="retrievers">A list of parameter retrievers to use to get values for the properties. If null, the parsed body will be used instead</param>
		/// <returns>A new update operation for <see cref="TModel"/></returns>
		IOperation<TModel, TUser> GetUpdateOperation(PropertyInfo[] properties, ParameterRetriever[]? retrievers);

		/// <summary>
		///		Gets a delete <see cref="IOperation{TModel, TUser}"/> for this model
		/// </summary>
		/// <returns>A new delete operation for <see cref="TModel"/></returns>
		IOperation<TModel, TUser> GetDeleteOperation();
	}
}