// -----------------------------------------------------------------------
// <copyright file="CreateOperation.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.LinqToDB.Operations {
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::LinqToDB.Data;
	using Microsoft.Extensions.DependencyInjection;

	using Seltzr.Context;
	using Seltzr.Operations;

	/// <summary>
	///     An operation that will create a model in a LinqToDB context
	/// </summary>
	/// <typeparam name="TModel">The type of model to create</typeparam>
	/// <typeparam name="TContext">The type of LinqToDB database context to use</typeparam>
	public class CreateOperation<TModel, TContext> : IOperation<TModel>
		where TModel : class where TContext : DataConnection {
		/// <summary>
		///     Creates the models specified in the request body
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The filtered dataset to operate on</param>
		/// <returns>The affected models</returns>
		public virtual async Task<IEnumerable<TModel>> OperateAsync(
			IApiContext<TModel, object> context,
			IQueryable<TModel> dataset) {
			TContext DatabaseContext = context.Services.GetRequiredService<TContext>();
			TModel[] Models = context.Parsed.Select(p => p.ParsedModel).ToArray();

			DatabaseContext.GetTable<TModel>().BulkCopy(Models);
			return Models;
		}
	}
}