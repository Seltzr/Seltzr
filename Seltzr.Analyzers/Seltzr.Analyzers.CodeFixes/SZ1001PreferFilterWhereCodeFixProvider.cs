// -----------------------------------------------------------------------
// <copyright file="SeltzrAnalyzersCodeFixProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Analyzers {
	using System.Collections.Immutable;
	using System.Composition;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CodeFixes;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SZ1001PreferFilterWhereCodeFixProvider))]
	[Shared]
	public class SZ1001PreferFilterWhereCodeFixProvider : BuilderMethodCodeFix {
		/// <summary>
		///     Initializes a new instance of the <see cref="SZ1001PreferFilterWhereCodeFixProvider" /> class.
		/// </summary>
		public SZ1001PreferFilterWhereCodeFixProvider()
			: base(SZ1001PreferFilterWhereAnalyzer.DiagnosticId, CodeFixResources.SZ1001Title) { }

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
			// Get the old callback first, and detect if it used the context parameter
			LambdaExpressionSyntax? OldCallback = invocation.GetArgument<LambdaExpressionSyntax>(0);
			bool UsingContext = OldCallback?.GetParameters().Count() == 2;
			InvocationExpressionSyntax? WhereCall =
				OldCallback?.GetSingleLambdaStatement() as InvocationExpressionSyntax;
			LambdaExpressionSyntax? NewCallback = WhereCall?.GetArgument<LambdaExpressionSyntax>(0);

			if (NewCallback != null && OldCallback != null && UsingContext) {
				// need to add context to new lambda
				ParameterSyntax? ElementParam = NewCallback.GetParameters().FirstOrDefault()
				                                ?? SeltzrSyntaxFactory.Parameter("_");
				ParameterSyntax ApiParam = OldCallback.GetParameters().FirstOrDefault();
				NewCallback = NewCallback.WithParameters(ApiParam, ElementParam);
			}

			InvocationExpressionSyntax Replacement = invocation.WithMethodName("FilterWhere")
				.WithArguments(NewCallback ?? WhereCall?.GetArgument(0)!);
			SyntaxNode SyntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
			SyntaxNode ReplacementRoot = SyntaxRoot.ReplaceNode(invocation, Replacement);

			return document.WithSyntaxRoot(ReplacementRoot);
		}
	}
}