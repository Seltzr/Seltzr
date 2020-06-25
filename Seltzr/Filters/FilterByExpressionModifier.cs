// -----------------------------------------------------------------------
// <copyright file="FilterByExpressionModifier.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Filters {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	/// <summary>
	///     Expression modifier for the FilterBy methods that strips away the parsed parameter and replaces it with a constant
	///     value. Additionally, it replaces the property parameter with the model itself and all references to it with
	///     references on the model.
	/// </summary>
	/// <typeparam name="TModel">The type of model to replace the parameter with</typeparam>
	/// <typeparam name="TParam">The type of the parameter to strip away</typeparam>
	internal class FilterByExpressionModifier<TModel, TParam> : ExpressionVisitor {
		/// <summary>
		///     The type of the parameter to strip away
		/// </summary>
		private readonly Type Param = typeof(TParam);

		/// <summary>
		///     A context expression that points to the <typeparamref name="TParam" /> value to use as a constant
		/// </summary>
		private readonly ConstantExpression ParameterConstant;

		/// <summary>
		///     The property that models are being filtered by
		/// </summary>
		private readonly PropertyInfo PropertyInfo;

		/// <summary>
		///     The name of the parameter that refers to the parsed request parameter
		/// </summary>
		private string? ParsedParamName;

		/// <summary>
		///     An expression to access the property that uses the new model parameter
		/// </summary>
		private MemberExpression? PropertyAccessExpression;

		/// <summary>
		///     The name of the parameter that refers to the model's property
		/// </summary>
		private string? PropertyParamName;

		/// <summary>Initializes a new instance of the <see cref="FilterByExpressionModifier{TModel,TParam}" /> class.</summary>
		/// <param name="property">The property being filtered by</param>
		/// <param name="param">The value of the parameter to use as a constant</param>
		public FilterByExpressionModifier(PropertyInfo property, TParam param) {
			this.ParameterConstant = Expression.Constant(param);
			this.PropertyInfo = property;
		}

		/// <summary>
		///     Modifies a given expression to strip away the <typeparamref name="TParam" /> parameter
		/// </summary>
		/// <param name="expression">The expression to modify</param>
		/// <returns>The modified expression</returns>
		public Expression<Func<TModel, bool>> Modify(Expression<Func<TParam, TParam, bool>> expression) {
			this.PropertyParamName = expression.Parameters[0].Name;
			this.ParsedParamName = expression.Parameters[1].Name;
			ParameterExpression ModelParameter = Expression.Parameter(typeof(TModel));
			this.PropertyAccessExpression = Expression.Property(ModelParameter, this.PropertyInfo);

			return Expression.Lambda<Func<TModel, bool>>(this.Visit(expression.Body), ModelParameter);
		}

		/// <summary>
		///     Visits a <see cref="ParameterExpression" />
		/// </summary>
		/// <param name="node">The <see cref="ParameterExpression" /> to visit</param>
		/// <returns>An expression to be used in the place of the given <see cref="ParameterExpression" /></returns>
		protected override Expression VisitParameter(ParameterExpression node) {
			if (node.Name == this.ParsedParamName) return this.ParameterConstant;
			if (node.Name == this.PropertyParamName) return this.PropertyAccessExpression!;
			return base.VisitParameter(node); // likely impossible but just in case
		}
	}
}