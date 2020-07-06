// -----------------------------------------------------------------------
// <copyright file="SZ1003SimplifyFiltersConditionsAnalyzer.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Analyzers {
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;

	/// <summary>
	///     Analyzer for rule SZ1003 that recommends simplifying common filter and condition builder methods
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SZ1003SimplifyFiltersConditionsAnalyzer : BuilderAnalyzer {
		/// <summary>
		///     The id for the diagnostic reported by this class
		/// </summary>
		public const string DiagnosticId = "SZ1003";

		/// <summary>
		///     Gets a list of the builder methods supported by this analyzer
		/// </summary>
		private static readonly string[] SupportedMethods = {
			                                                    "Limit", "RequireExactly", "RequireInputHasExactly",
			                                                    "RequireAtLeast", "RequireInputHasAtLeast"
															};

		/// <summary>
		///     Initializes a new instance of the <see cref="SZ1003SimplifyFiltersConditionsAnalyzer" /> class.
		/// </summary>
		public SZ1003SimplifyFiltersConditionsAnalyzer()
			: base(
				SZ1003SimplifyFiltersConditionsAnalyzer.DiagnosticId,
				"Usage",
				nameof(Resources.SZ1003Title),
				nameof(Resources.SZ1003Message),
				nameof(Resources.SZ1003Description),
				DiagnosticSeverity.Info) { }

		/// <summary>
		///     Analyzes an invocation expression syntax node
		/// </summary>
		/// <param name="context">The analyzer context</param>
		protected override void AnalyzeNode(SyntaxNodeAnalysisContext context) {
			InvocationExpressionSyntax InvocationExpression = (InvocationExpressionSyntax)context.Node;

			if (!BuilderAnalyzer.IsBuilderMethod(
				    InvocationExpression,
				    SZ1003SimplifyFiltersConditionsAnalyzer.SupportedMethods,
				    context.SemanticModel)) return;

			// get the first parameter and test if its 1
			ExpressionSyntax? ValueArg = InvocationExpression?.GetArgument(0);
			Optional<object> Value = context.SemanticModel.GetConstantValue(ValueArg);
			if (!Value.HasValue) return;
			if (!(Value.Value is int ActualValue)) return;
			if (ActualValue != 1) return;

			this.ReportDiagnostic(context, InvocationExpression?.GetMethodName() ?? "this method");
		}
	}
}