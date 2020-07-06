// -----------------------------------------------------------------------
// <copyright file="SZ1002UnusedContextAnalyzer.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Analyzers {
	using System.Collections.Immutable;
	using System.Linq;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;

	/// <summary>
	///     Analyzer for rule SZ1002 that recommends the usage of a builder method overload without IApiContext, since that parameter is unused by the lambda
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SZ1002UnusedContextAnalyzer : BuilderAnalyzer {
		/// <summary>
		///		The id for the diagnostic reported by this class
		/// </summary>
		public const string DiagnosticId = "SZ1002";

		/// <summary>
		///		A diagnostic property key that points to the index of the unused context parameter in the parameter list
		/// </summary>
		public const string ContextParamIndexKey = "SZ1002UnusedContextAnalyzer.ContextParamIndexKey";

		/// <summary>
		///		Gets a list of the builder methods supported by this analyzer
		/// </summary>
		private static readonly string[] SupportedMethods = { "Filter", "FilterWhere", "Require", "RequireAsync", "FilterAsync" };

		/// <summary>
		///     Initializes a new instance of the <see cref="SZ1002UnusedContextAnalyzer" /> class.
		/// </summary>
		public SZ1002UnusedContextAnalyzer()
			: base(
				SZ1002UnusedContextAnalyzer.DiagnosticId,
				"Usage",
				nameof(Resources.SZ1002Title),
				nameof(Resources.SZ1002Message),
				nameof(Resources.SZ1002Description)) { }

		/// <summary>
		///     Analyzes an invocation expression syntax node
		/// </summary>
		/// <param name="context">The analyzer context</param>
		protected override void AnalyzeNode(SyntaxNodeAnalysisContext context) {
			InvocationExpressionSyntax InvocationExpression = (InvocationExpressionSyntax)context.Node;

			if (!BuilderAnalyzer.IsBuilderMethod(InvocationExpression, SZ1002UnusedContextAnalyzer.SupportedMethods, context.SemanticModel)) return;

			// okay the method has overloads with and without IApiContext
			LambdaExpressionSyntax? LambdaExpression = InvocationExpression.GetArgument<LambdaExpressionSyntax>();
			IMethodSymbol? Info = context.SemanticModel.GetSymbolInfo(InvocationExpression).Symbol as IMethodSymbol;
			IParameterSymbol? LambdaParam = Info?.Parameters.FirstOrDefault(p => p.Type.Name == "Func");
			int ContextParamIndex = (LambdaParam?.Type as INamedTypeSymbol)?.TypeArguments.ToList()
				.FindIndex(i => i.MetadataName == "IApiContext`2") ?? -1;

			if (ContextParamIndex == -1) return;
			DataFlowAnalysis Analysis =
				context.SemanticModel.AnalyzeDataFlow(LambdaExpression);
			bool IsContextUsed = Analysis.ReadInside.OfType<IParameterSymbol>().Any(p => p.Type.MetadataName == "IApiContext`2");

			if (IsContextUsed) return;

			ImmutableDictionary<string, string>.Builder PropBuilder = ImmutableDictionary.CreateBuilder<string, string>();
			PropBuilder.Add(SZ1002UnusedContextAnalyzer.ContextParamIndexKey, ContextParamIndex.ToString());

			this.ReportDiagnostic(context, InvocationExpression, PropBuilder.ToImmutable());
		}
	}
}