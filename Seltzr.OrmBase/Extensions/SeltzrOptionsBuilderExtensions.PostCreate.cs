// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilderExtensions.PostCreate.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace - keep it all together
namespace Seltzr.Extensions {
	using System;

	using Seltzr.Exceptions;
	using Seltzr.Operations;
	using Seltzr.Options.Builder;
	using Seltzr.OrmBase.Options;

	/// <summary>
	///     Extension methods for the <see cref="SeltzrOptionsBuilder{TModel,TUser}" /> class
	/// </summary>
	public static partial class SeltzrOptionsBuilderExtensions {
		/// <summary>
		///     Sets this route up to handle a POST creation request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> PostCreate<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			if (!(builder is IOrmSeltzrOptionsBuilder<TModel, TUser> OrmBuilder))
				throw new OptionsException(
					$"PostCreate may only be called on a builder that inherits from {nameof(IOrmSeltzrOptionsBuilder<TModel, TUser>)}");

			IOperation<TModel, TUser> CreateOperation = OrmBuilder.GetCreateOperation();
			return builder.SetupPost(
				routePattern,
				o => {
					o.IgnorePrimaryKey();
					o.UseOperation(CreateOperation);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a POST creation request with the same route pattern using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> PostCreate<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			Action<SeltzrOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PostCreate(string.Empty, optionsHandler);
	}
}