// -----------------------------------------------------------------------
// <copyright file="SZ1002UnusedContextCodeFixProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------


namespace Seltzr.Analyzers {
	using System;
	using System.Collections.Immutable;
	using System.Composition;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CodeFixes;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	/// <summary>
	///		Code fix for SZ1002 that removes the unused context parameter
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SZ1002UnusedContextCodeFixProvider))]
	[Shared]
	public class SZ1002UnusedContextCodeFixProvider : BuilderMethodCodeFix {
		/// <summary>
		///     Initializes a new instance of the <see cref="SZ1002UnusedContextCodeFixProvider" /> class.
		/// </summary>
		public SZ1002UnusedContextCodeFixProvider()
			: base(SZ1002UnusedContextAnalyzer.DiagnosticId, CodeFixResources.SZ1002Title) { }

		/// <summary>
		///     Transforms the given document to fix the diagnostic issue
		/// </summary>
		/// <param name="document">The input document to transform</param>
		/// <param name="invocation">The builder method to fix</param>
		/// <param name="cancellationToken">A cancellation token to use to cancel the operation</param>
		/// <param name="properties">Any extra diagnostic-specific properties</param>
		/// <returns>The transformed document</returns>
		protected override async Task<Document> FixBuilderMethod(
			Document document,
			InvocationExpressionSyntax invocation,
			CancellationToken cancellationToken,
			ImmutableDictionary<string, string> properties) {
			LambdaExpressionSyntax? Lambda = invocation.GetArgument<LambdaExpressionSyntax>();
			if (Lambda == null) return document;

			// skip the context parameter
			int Index = Int32.Parse(properties[SZ1002UnusedContextAnalyzer.ContextParamIndexKey]);
			Lambda = Lambda.WithParameters(Lambda.GetParameters().ToList().Where((p, i) => i != Index).ToArray());

			// replace the argument
			InvocationExpressionSyntax Replacement = invocation.SwapArgument(Lambda);

			// and replace
			SyntaxNode SyntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
			SyntaxNode ReplacementRoot = SyntaxRoot.ReplaceNode(invocation, Replacement);

			return document.WithSyntaxRoot(ReplacementRoot);
		}
	}
}