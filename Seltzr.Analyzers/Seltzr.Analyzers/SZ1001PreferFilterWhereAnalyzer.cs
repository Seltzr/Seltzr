// -----------------------------------------------------------------------
// <copyright file="SZ1001PreferFilterWhereAnalyzer.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Analyzers {
	using System.Linq;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;

	/// <summary>
	///     Analyzer for rule SZ1001 that discourages <c>builder.Filter(data => data.Where(expression))</c> in favor of
	///     <c>builder.FilterWhere(expression)</c>
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SZ1001PreferFilterWhereAnalyzer : BuilderAnalyzer {
		/// <summary>
		///		The id for the diagnostic reported by this class
		/// </summary>
		public const string DiagnosticId = "SZ1001";

		/// <summary>
		///     Initializes a new instance of the <see cref="SZ1001PreferFilterWhereAnalyzer" /> class.
		/// </summary>
		public SZ1001PreferFilterWhereAnalyzer()
			: base(
				SZ1001PreferFilterWhereAnalyzer.DiagnosticId,
				"Usage",
				nameof(Resources.SZ1001Title),
				nameof(Resources.SZ1001Message),
				nameof(Resources.SZ1001Description)) { }

		/// <summary>
		///     Analyzes an invocation expression syntax node
		/// </summary>
		/// <param name="context">The analyzer context</param>
		protected override void AnalyzeNode(SyntaxNodeAnalysisContext context) {
			InvocationExpressionSyntax InvocationExpression = (InvocationExpressionSyntax)context.Node;

			if (!BuilderAnalyzer.IsBuilderMethod(InvocationExpression, "Filter", context.SemanticModel)) return;

			// okay the method is options.filter, check if it uses d.Where
			if (InvocationExpression.ArgumentList.Arguments.Count != 1) return;
			LambdaExpressionSyntax? LambdaExpression = InvocationExpression.GetArgument<LambdaExpressionSyntax>(0);

			if (!(LambdaExpression?.GetSingleLambdaStatement() is InvocationExpressionSyntax WhereInvocation)) return;

			if (!BuilderAnalyzer.IsMethod(
				    WhereInvocation,
				    nameof(Queryable.Where),
				    typeof(Queryable).FullName,
				    context.SemanticModel)) return;

			// .where is the only statement, warn
			string? Name = InvocationExpression.GetMethodMemberName();
			this.ReportDiagnostic(context,  Name ?? "builder");
		}
	}
}