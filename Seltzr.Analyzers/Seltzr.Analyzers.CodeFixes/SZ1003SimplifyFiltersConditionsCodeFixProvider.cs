// -----------------------------------------------------------------------
// <copyright file="SZ1003SimplifyFiltersConditionsCodeFixProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Analyzers {
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Composition;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CodeFixes;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	/// <summary>
	///     Code fix for SZ1003 that replaces certain builder methods with a simplified version
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SZ1003SimplifyFiltersConditionsCodeFixProvider))]
	[Shared]
	public class SZ1003SimplifyFiltersConditionsCodeFixProvider : BuilderMethodCodeFix {
		/// <summary>
		///     Lookup dictionary for the replacement values for builder methods handled by this code fix.
		/// </summary>
		private static readonly Dictionary<string, string> AnalogousMethodLookup = new Dictionary<string, string> {
			                                                                                                          { "Limit", "LimitOne" },
			                                                                                                          { "RequireExactly", "RequireExactlyOne" },
			                                                                                                          { "RequireAtLeast", "RequireNonEmpty" },
			                                                                                                          { "RequireInputHasExactly", "RequireExactlyOneInput" },
			                                                                                                          { "RequireInputHasAtLeast", "RequireNonEmptyInput" }
		                                                                                                          };

		/// <summary>
		///     Initializes a new instance of the <see cref="SZ1003SimplifyFiltersConditionsCodeFixProvider" /> class.
		/// </summary>
		public SZ1003SimplifyFiltersConditionsCodeFixProvider()
			: base(SZ1003SimplifyFiltersConditionsAnalyzer.DiagnosticId, CodeFixResources.SZ1003Title) { }

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
			// just skip the first argument and rename the method
			InvocationExpressionSyntax Replacement = invocation
				.WithArguments(invocation.ArgumentList.Arguments.Skip(1).Select(x => x.Expression).ToArray())
				.WithMethodName(SZ1003SimplifyFiltersConditionsCodeFixProvider.AnalogousMethodLookup[invocation.GetMethodName()!]);
			SyntaxNode SyntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
			SyntaxNode ReplacementRoot = SyntaxRoot.ReplaceNode(invocation, Replacement);

			return document.WithSyntaxRoot(ReplacementRoot);
		}
	}
}