// -----------------------------------------------------------------------
// <copyright file="FilterWhereExpressionModifier.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Filters {
	using System;
	using System.Linq.Expressions;

	using Seltzr.Context;
	using Seltzr.Options.Builder;

	/// <summary>
	///     Expression modifier for the
	///     <see
	///         cref="SeltzrOptionsBuilder{TModel,TUser}.FilterWhere(System.Linq.Expressions.Expression{Func{Context.IApiContext{TModel, TUser}, TModel, bool}})" />
	///     method that strips away the API context argument so the expression can still be translated into SQL.
	/// </summary>
	/// <typeparam name="TModel">The type of model being filtered</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	internal class FilterWhereExpressionModifier<TModel, TUser> : ExpressionVisitor
		where TModel : class where TUser : class {
		/// <summary>
		///     The type of the <see cref="IApiContext{TModel, TUser}" />
		/// </summary>
		private readonly Type ApiType = typeof(IApiContext<TModel, TUser>);

		/// <summary>
		///     A context expression that points to the <see cref="IApiContext{TModel, TUser}" /> of the request
		/// </summary>
		private readonly ConstantExpression ContextExpression;

		/// <summary>Initializes a new instance of the <see cref="FilterWhereExpressionModifier{TModel,TUser}" /> class.</summary>
		/// <param name="apiContext">The api context of the request</param>
		public FilterWhereExpressionModifier(IApiContext<TModel, TUser> apiContext) =>
			this.ContextExpression = Expression.Constant(apiContext);

		/// <summary>
		///     Modifies a given expression to strip away the <see cref="IApiContext{TModel, TUser}" /> parameter
		/// </summary>
		/// <param name="expression">The expression to modify</param>
		/// <returns>The modified expression</returns>
		public Expression<Func<TModel, bool>> Modify(
			Expression<Func<IApiContext<TModel, TUser>, TModel, bool>> expression) =>
			Expression.Lambda<Func<TModel, bool>>(this.Visit(expression.Body), expression.Parameters[1]);

		/// <summary>
		///     Visits a <see cref="ParameterExpression" />
		/// </summary>
		/// <param name="node">The <see cref="ParameterExpression" /> to visit</param>
		/// <returns>An expression to be used in the place of the given <see cref="ParameterExpression" /></returns>
		protected override Expression VisitParameter(ParameterExpression node) =>
			node.Type == this.ApiType ? this.ContextExpression : base.VisitParameter(node);
	}
}