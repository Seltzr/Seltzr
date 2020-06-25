// -----------------------------------------------------------------------
// <copyright file="EFCoreSeltzrOptionsBuilderExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace Seltzr.Extensions {
	using System;
	using System.Linq.Expressions;

	using Microsoft.EntityFrameworkCore;

	using Seltzr.Options.Builder;

	/// <summary>
	///     Extension methods for the <see cref="SeltzrOptionsBuilder{TModel,TUser}" /> class specific to Entity Framework
	/// </summary>
	public static class EFCoreSeltzrOptionsBuilderExtensions {
		/// <summary>
		///     Eagerly loads the given properties before returning the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="properties">The properties to eagerly load</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel,TUser}" /> object, for chaining</returns>
		/// <remarks>
		///     This method internally calls Entity Framework's <c>Include</c> method. However, calls to <c>ThenInclude</c> are unsupported. Instead, use <see cref="SeltzrOptionsBuilder{TModel, TUser}.Filter(Func{System.Linq.IQueryable{TModel}, System.Linq.IQueryable{TModel}})"/> to include multiple levels of navigation properties.
		/// </remarks>
		/// <example>
		///		Include navigation properties in the API response:
		///		<code>
		///			options.EagerLoad(e => e.Project, e => e.Profile);
		///		</code>
		///		Include all projects and tasks in the API response:
		///		<code>
		///			options.Filter(d => d.Include(e => e.Project).ThenInclude(p => p.Tasks));
		///		</code>	
		/// </example>
		public static SeltzrOptionsBuilder<TModel, TUser> EagerLoad<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			params Expression<Func<TModel, object>>[] properties)
			where TModel : class where TUser : class {
			foreach (Expression<Func<TModel, object>> Property in properties)
				builder.Filter(d => d.Include(Property));
			return builder;
		}
	}
}