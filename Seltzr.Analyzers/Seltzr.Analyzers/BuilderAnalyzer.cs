// -----------------------------------------------------------------------
// <copyright file="BuilderAnalyzer.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Analyzers {
	using System.Collections.Immutable;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;
	using Microsoft.CodeAnalysis.Text;

	/// <summary>
	///     Base class for analyzers that work on a SeltzrOptionsBuilder method.
	/// </summary>
	public abstract class BuilderAnalyzer : DiagnosticAnalyzer {
		/// <summary>
		///     The rule this analyzer will report if matched
		/// </summary>
		private readonly DiagnosticDescriptor Rule;

		/// <summary>
		///     Initializes a new instance of the <see cref="BuilderAnalyzer" /> class.
		/// </summary>
		/// <param name="diagnosticId">The diagnostic ID to report, e.g. "SZ0000"</param>
		/// <param name="category">The category the diagnostic falls under</param>
		/// <param name="titleResource">The name of the localizable diagnostic title resource to use</param>
		/// <param name="messageResource">The name of the localizable diagnostic message resource to use</param>
		/// <param name="descriptionResource">The name of the localizable diagnostic description resource to use</param>
		/// <param name="severity">The severity of the diagnostic</param>
		protected BuilderAnalyzer(
			string diagnosticId,
			string category,
			string titleResource,
			string messageResource,
			string descriptionResource,
			DiagnosticSeverity severity = DiagnosticSeverity.Warning) {
			LocalizableString Title = new LocalizableResourceString(
				titleResource,
				Resources.ResourceManager,
				typeof(Resources));
			LocalizableString Message = new LocalizableResourceString(
				messageResource,
				Resources.ResourceManager,
				typeof(Resources));
			LocalizableString Description = new LocalizableResourceString(
				descriptionResource,
				Resources.ResourceManager,
				typeof(Resources));
			this.Rule = new DiagnosticDescriptor(
				diagnosticId,
				Title,
				Message,
				category,
				severity,
				true,
				Description);
		}

		/// <summary>
		///     Gets a list of rules that could be reported by this analyzer
		/// </summary>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this.Rule);

		/// <summary>
		///     Initializes the analyzer and registers a callback for invocation expressions.
		/// </summary>
		/// <param name="context">The current analysis context</param>
		public override void Initialize(AnalysisContext context) {
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(this.AnalyzeNode, SyntaxKind.InvocationExpression);
		}

		/// <summary>
		///     Gets a location in source code for the invocation expression
		/// </summary>
		/// <param name="invocationExpression">The expression to get the location for</param>
		/// <returns>
		///     The location of the invocation expression, from after the dereference operator (".") to the end of the
		///     parameter list
		/// </returns>
		protected static Location GetInvocationLocation(InvocationExpressionSyntax invocationExpression) {
			// start needs to be adjusted to after the ., end can stay the same
			MemberAccessExpressionSyntax? MemberAccessExpressionSyntax =
				invocationExpression.Expression as MemberAccessExpressionSyntax;
			IdentifierNameSyntax? IdentifierNameSyntax = invocationExpression.Expression as IdentifierNameSyntax;
			int Start = MemberAccessExpressionSyntax?.Name?.SpanStart
			            ?? IdentifierNameSyntax?.Identifier.SpanStart ?? invocationExpression.SpanStart;
			int End = invocationExpression.Span.End;

			return Location.Create(invocationExpression.SyntaxTree, new TextSpan(Start, End - Start));
		}

		/// <summary>
		///     Gets whether or not the provided invocation syntax refers to the given method on the
		///     <c>Seltzr.Options.Builder.SeltzrOptionsBuilder</c> type
		/// </summary>
		/// <param name="invocationExpression">The invocation expression syntax to compare against the given builder method</param>
		/// <param name="methodName">
		///     The name of the builder method to compare with <paramref name="invocationExpression" />
		/// </param>
		/// <param name="model">The semantic model for the compilation</param>
		/// <returns><see langword="true" /> if the methods match, <see langword="false" /> otherwise</returns>
		protected static bool IsBuilderMethod(
			InvocationExpressionSyntax invocationExpression,
			string methodName,
			SemanticModel model) =>
			BuilderAnalyzer.IsBuilderMethod(
				invocationExpression,
				new string[] { methodName },
				model);

		/// <summary>
		///     Gets whether or not the provided invocation syntax refers to any of the given methods on the
		///     <c>Seltzr.Options.Builder.SeltzrOptionsBuilder</c> type
		/// </summary>
		/// <param name="invocationExpression">The invocation expression syntax to compare against the given builder method</param>
		/// <param name="methodNames">
		///     The names of the builder method to compare with <paramref name="invocationExpression" />
		/// </param>
		/// <param name="model">The semantic model for the compilation</param>
		/// <returns><see langword="true" /> if the methods match, <see langword="false" /> otherwise</returns>
		protected static bool IsBuilderMethod(
			InvocationExpressionSyntax invocationExpression,
			string[] methodNames,
			SemanticModel model) =>
			BuilderAnalyzer.IsMethod(
				invocationExpression,
				methodNames,
				"Seltzr.Options.Builder.SeltzrOptionsBuilder`2",
				model);

		/// <summary>
		///     Gets whether or not the provided invocation syntax refers to any of the given methods
		/// </summary>
		/// <param name="invocationExpression">The invocation expression syntax to compare against the given method</param>
		/// <param name="methodNames">The names of the method to compare with <paramref name="invocationExpression" /></param>
		/// <param name="containerTypeMetadataName">
		///     The fully-qualified metadata name of the type containing the method to compare
		///     with <paramref name="invocationExpression" />
		/// </param>
		/// <param name="model">The semantic model for the compilation</param>
		/// <returns><see langword="true" /> if the methods match, <see langword="false" /> otherwise</returns>
		protected static bool IsMethod(
			InvocationExpressionSyntax invocationExpression,
			string[] methodNames,
			string containerTypeMetadataName,
			SemanticModel model) {
			if (methodNames.All(n => n != invocationExpression.GetMethodName())) return false;
			if (!(model.GetSymbolInfo(invocationExpression).Symbol is IMethodSymbol SymbolInfo)) return false;

			INamedTypeSymbol ExpectedType = model.Compilation.GetTypeByMetadataName(containerTypeMetadataName);
			INamedTypeSymbol ActualType = SymbolInfo.ContainingType;
			return SymbolEqualityComparer.Default.Equals(
				ActualType.IsGenericType ? ActualType.ConstructedFrom : ActualType,
				ExpectedType);
		}

		/// <summary>
		///     Gets whether or not the provided invocation syntax refers to the given method
		/// </summary>
		/// <param name="invocationExpression">The invocation expression syntax to compare against the given method</param>
		/// <param name="methodName">The name of the method to compare with <paramref name="invocationExpression" /></param>
		/// <param name="containerTypeMetadataName">
		///     The fully-qualified metadata name of the type containing the method to compare
		///     with <paramref name="invocationExpression" />
		/// </param>
		/// <param name="model">The semantic model for the compilation</param>
		/// <returns><see langword="true" /> if the methods match, <see langword="false" /> otherwise</returns>
		protected static bool IsMethod(
			InvocationExpressionSyntax invocationExpression,
			string methodName,
			string containerTypeMetadataName,
			SemanticModel model) {
			return BuilderAnalyzer.IsMethod(invocationExpression, new[] { methodName }, containerTypeMetadataName, model);
		}

		/// <summary>
		///     Analyzes an invocation expression syntax node
		/// </summary>
		/// <param name="context">The analyzer context</param>
		protected abstract void AnalyzeNode(SyntaxNodeAnalysisContext context);

		/// <summary>
		///     Reports the diagnostic for this analyzer, using the invocation expression provided by the analysis context
		/// </summary>
		/// <param name="context">The context to report the diagnostic on</param>
		/// <param name="formatArgs">Arguments for formatting the message string</param>
		protected void ReportDiagnostic(SyntaxNodeAnalysisContext context, params object[] formatArgs) {
			this.ReportDiagnostic(context, (InvocationExpressionSyntax)context.Node, formatArgs);
		}

		/// <summary>
		///     Reports the diagnostic for this analyzer
		/// </summary>
		/// <param name="context">The context to report the diagnostic on</param>
		/// <param name="invocationExpression">The invocation expression to report at</param>
		/// <param name="formatArgs">Arguments for formatting the message string</param>
		protected void ReportDiagnostic(
			SyntaxNodeAnalysisContext context,
			InvocationExpressionSyntax invocationExpression,
			params object[] formatArgs) =>
			context.ReportDiagnostic(
				Diagnostic.Create(this.Rule, BuilderAnalyzer.GetInvocationLocation(invocationExpression), formatArgs));

		/// <summary>
		///     Reports the diagnostic for this analyzer
		/// </summary>
		/// <param name="context">The context to report the diagnostic on</param>
		/// <param name="invocationExpression">The invocation expression to report at</param>
		/// <param name="properties">Additional properties for the diagnostic</param>
		/// <param name="formatArgs">Arguments for formatting the message string</param>
		protected void ReportDiagnostic(
			SyntaxNodeAnalysisContext context,
			InvocationExpressionSyntax invocationExpression,
			ImmutableDictionary<string, string> properties,
			params object[] formatArgs) {
			context.ReportDiagnostic(
				Diagnostic.Create(this.Rule, BuilderAnalyzer.GetInvocationLocation(invocationExpression), properties, formatArgs));
		}
	}
}