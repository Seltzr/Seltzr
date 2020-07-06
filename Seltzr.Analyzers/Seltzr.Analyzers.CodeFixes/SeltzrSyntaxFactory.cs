// -----------------------------------------------------------------------
// <copyright file="SeltzrSyntaxFactory.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Analyzers {
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	/// <summary>
	///     Methods that create syntax nodes for transforming documents
	/// </summary>
	public static class SeltzrSyntaxFactory {
		/// <summary>
		///     Creates a new <see cref="ParameterSyntax" /> with the given identifier name.
		/// </summary>
		/// <param name="identifier">The name of the identifier for the parameter</param>
		/// <returns>The new <see cref="ParameterSyntax" /></returns>
		public static ParameterSyntax Parameter(string identifier) =>
			SyntaxFactory.Parameter(SeltzrSyntaxFactory.Token(identifier));

		/// <summary>
		///     Creates a new <see cref="SyntaxToken" /> with the given text and value text
		/// </summary>
		/// <param name="valueText">The text and value text of the token</param>
		/// <returns>The new <see cref="SyntaxToken" /></returns>
		public static SyntaxToken Token(string valueText) =>
			SyntaxFactory.Token(default, SyntaxKind.IdentifierToken, valueText, valueText, default);
	}
}