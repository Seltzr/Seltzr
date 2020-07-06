// -----------------------------------------------------------------------
// <copyright file="BuilderMethodCodeFix.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Analyzers {
	using System.Collections.Immutable;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CodeActions;
	using Microsoft.CodeAnalysis.CodeFixes;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Text;

	/// <summary>
	///     Base class for code fixes that fix builder methods
	/// </summary>
	public abstract class BuilderMethodCodeFix : CodeFixProvider {
		/// <summary>
		///     The title of the code fix
		/// </summary>
		private readonly string Title;

		/// <summary>
		///		Initializes a new instance of the <see cref="BuilderMethodCodeFix" /> class
		/// </summary>
		/// <param name="diagnosticId">The id of the diagnostic this code fix handles</param>
		/// <param name="title">The title of the code fix</param>
		protected BuilderMethodCodeFix(string diagnosticId, string title) {
			this.FixableDiagnosticIds = ImmutableArray.Create(diagnosticId);
			this.Title = title;
		}

		/// <summary>
		///     Gets the ids of diagnostics this code fix can handle
		/// </summary>
		public override sealed ImmutableArray<string> FixableDiagnosticIds { get; }

		/// <summary>
		///     Gets the provider to use to fix all occurrences of the diagnostic this code fix handles
		/// </summary>
		/// <returns>The <see cref="FixAllProvider" /> to use for this code fix</returns>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		/// <summary>
		///     Registers code fixes for this class
		/// </summary>
		/// <param name="context">The current context for code fixes</param>
		/// <returns>When all code fixes for this class have been registered</returns>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
			SyntaxNode Root =
				await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			Diagnostic Diagnostic = context.Diagnostics.First();
			TextSpan DiagnosticSpan = Diagnostic.Location.SourceSpan;

			// Find the type declaration identified by the diagnostic.
			InvocationExpressionSyntax BuilderMethod = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf()
				.OfType<InvocationExpressionSyntax>().First();

			// Register a code action that will invoke the fix.
			context.RegisterCodeFix(
				CodeAction.Create(
					this.Title,
					c => this.FixBuilderMethod(context.Document, BuilderMethod, c, Diagnostic.Properties),
					this.FixableDiagnosticIds[0]),
				Diagnostic);
		}

		/// <summary>
		///     Transforms the given document to fix the diagnostic issue
		/// </summary>
		/// <param name="document">The input document to transform</param>
		/// <param name="invocation">The builder method to fix</param>
		/// <param name="cancellationToken">A cancellation token to use to cancel the operation</param>
		/// <param name="properties">Any extra diagnostic-specific properties</param>
		/// <returns>The transformed document</returns>
		protected abstract Task<Document> FixBuilderMethod(
			Document document,
			InvocationExpressionSyntax invocation,
			CancellationToken cancellationToken,
			ImmutableDictionary<string, string> properties);
	}
}