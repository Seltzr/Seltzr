﻿// -----------------------------------------------------------------------
// <copyright file="Linq2DBModelProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.LinqToDB {
	using System.Linq;
	using System.Threading.Tasks;

	using global::LinqToDB.Data;

	using Microsoft.Extensions.DependencyInjection;

	using Seltzr.Context;
	using Seltzr.Models;

	/// <summary>
	///     Model provider that uses LinqToDB as a backend
	/// </summary>
	/// <typeparam name="TModel">The type of model whose dataset to provide</typeparam>
	/// <typeparam name="TContext">The type of the database context to use</typeparam>
	public class LinqToDBModelProvider<TModel, TContext> : IModelProvider<TModel>
		where TModel : class where TContext : DataConnection {
		/// <summary>
		///     Gets a LinqToDB query pointing to all of the models available for the current request context
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>An <see cref="IQueryable{T}" /> of all of the models available</returns>
		public virtual async Task<IQueryable<TModel>> GetModelsAsync(IApiContext<TModel, object> context) {
			TContext DatabaseContext = context.Services.GetRequiredService<TContext>();
			return DatabaseContext.GetTable<TModel>();
		}
	}
}