// -----------------------------------------------------------------------
// <copyright file="InvocationExpressionSyntaxExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Analyzers {
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	/// <summary>
	///     Extension methods for the <see cref="ExpressionSyntax" /> family of classes.
	/// </summary>
	public static class ExpressionSyntaxExtensions {
		/// <summary>
		///     Gets the first argument of the invocation expression with the specified type
		/// </summary>
		/// <typeparam name="TSyntax">The type of the argument</typeparam>
		/// <param name="invocation">The invocation expression to get the argument from</param>
		/// <returns>The argument syntax expression, or null if no argument matches the type</returns>
		public static TSyntax? GetArgument<TSyntax>(this InvocationExpressionSyntax invocation)
			where TSyntax : ExpressionSyntax {
			return invocation.ArgumentList.Arguments.FirstOrDefault(a => a.Expression is TSyntax)?.Expression as TSyntax;
		}

		/// <summary>
		///     Gets the index of the first argument of the invocation expression with the specified type
		/// </summary>
		/// <typeparam name="TSyntax">The type of the argument</typeparam>
		/// <param name="invocation">The invocation expression to get the argument from</param>
		/// <returns>The index of the argument syntax expression, or -1 if no argument matches the type</returns>
		public static int GetArgumentIndex<TSyntax>(this InvocationExpressionSyntax invocation)
			where TSyntax : ExpressionSyntax {
			return invocation.ArgumentList.Arguments.ToList().FindIndex(a => a.Expression is TSyntax);
		}

		/// <summary>
		///     Swaps the first argument of the given type with the given expression syntax
		/// </summary>
		/// <typeparam name="TSyntax">The type of the argument to swap</typeparam>
		/// <param name="invocation">The invocation expression to get the argument from</param>
		/// <param name="replacement">The replacement argument</param>
		/// <returns>A new <see cref="InvocationExpressionSyntax"/> with the swapped argument</returns>
		public static InvocationExpressionSyntax SwapArgument<TSyntax>(this InvocationExpressionSyntax invocation, TSyntax replacement)
			where TSyntax : ExpressionSyntax {
			List<ArgumentSyntax> Existing = invocation.ArgumentList.Arguments.ToList();
			int Index = Existing.FindIndex(a => a.Expression is TSyntax);
			Existing[Index] = SyntaxFactory.Argument(replacement);
			return invocation.WithArgumentList(
				invocation.ArgumentList.WithArguments(SyntaxFactory.SeparatedList(Existing)));
		}

		/// <summary>
		///     Gets an argument of the invocation expression
		/// </summary>
		/// <typeparam name="TSyntax">The type of the argument</typeparam>
		/// <param name="invocation">The invocation expression to get the argument from</param>
		/// <param name="index">The index of the argument to get in the argument list</param>
		/// <returns>The argument syntax expression</returns>
		public static TSyntax? GetArgument<TSyntax>(this InvocationExpressionSyntax invocation, int index)
			where TSyntax : ExpressionSyntax =>
			invocation.GetArgument(index) as TSyntax;

		/// <summary>
		///     Gets an argument of the invocation expression
		/// </summary>
		/// <param name="invocation">The invocation expression to get the argument from</param>
		/// <param name="index">The index of the argument to get in the argument list</param>
		/// <returns>The argument syntax expression</returns>
		public static ExpressionSyntax? GetArgument(this InvocationExpressionSyntax invocation, int index) =>
			invocation.ArgumentList.Arguments[index]?.Expression;

		/// <summary>
		///     Gets the name of the member a method was called on
		/// </summary>
		/// <param name="invocationExpression">The invocation expression to get the member name from</param>
		/// <returns>The name of the member the method was called on in the <paramref name="invocationExpression" /></returns>
		public static string? GetMethodMemberName(this InvocationExpressionSyntax invocationExpression) {
			MemberAccessExpressionSyntax? MemberAccessExpressionSyntax = invocationExpression.Expression as MemberAccessExpressionSyntax;

			// keep going until we hit the actual member
			while (MemberAccessExpressionSyntax?.Expression is InvocationExpressionSyntax InvokeChild) {
				MemberAccessExpressionSyntax = InvokeChild.Expression as MemberAccessExpressionSyntax;

			}
			return MemberAccessExpressionSyntax?.Expression.ToString();

		}

		/// <summary>
		///     Gets the unqualified method name from an invocation expression
		/// </summary>
		/// <param name="invocationExpression">The invocation expression to get the method name from</param>
		/// <returns>The unqualified name of the method called in the <paramref name="invocationExpression" /></returns>
		public static string? GetMethodName(this InvocationExpressionSyntax invocationExpression) {
			MemberAccessExpressionSyntax? MemberAccessExpressionSyntax =
				invocationExpression.Expression as MemberAccessExpressionSyntax;
			IdentifierNameSyntax? IdentifierNameSyntax = invocationExpression.Expression as IdentifierNameSyntax;
			string? MethodName = MemberAccessExpressionSyntax?.Name?.Identifier.ValueText
			                     ?? IdentifierNameSyntax?.Identifier.ValueText;
			return MethodName;
		}

		/// <summary>
		///     Changes the method name of an invocation expression
		/// </summary>
		/// <param name="invocationExpression">The invocation expression to change the method name of</param>
		/// <param name="newName">The new name of the method</param>
		/// <returns>A new <see cref="InvocationExpressionSyntax"/> with an updated name</returns>
		public static InvocationExpressionSyntax WithMethodName(this InvocationExpressionSyntax invocationExpression, string newName) {
			MemberAccessExpressionSyntax? MemberAccessExpressionSyntax =
				invocationExpression.Expression as MemberAccessExpressionSyntax;

			IdentifierNameSyntax NewNameSyntax = SyntaxFactory.IdentifierName(newName);
			return MemberAccessExpressionSyntax == null
				       ? invocationExpression.WithExpression(NewNameSyntax.WithTriviaFrom(invocationExpression.Expression))
				       : invocationExpression.WithExpression(MemberAccessExpressionSyntax?.WithName(NewNameSyntax));
		}

		/// <summary>
		///     Removes the arguments of an invocation expression
		/// </summary>
		/// <param name="invocationExpression">The invocation expression to remove the arguments of</param>
		/// <returns>A new <see cref="InvocationExpressionSyntax"/> with no arguments</returns>
		public static InvocationExpressionSyntax WithNoArguments(this InvocationExpressionSyntax invocationExpression) {
			// same thing, just slightly more descriptive method name
			return invocationExpression.WithArguments();
		}

		/// <summary>
		///     Changes the arguments of an invocation expression
		/// </summary>
		/// <param name="invocationExpression">The invocation expression to change the arguments of</param>
		/// <param name="arguments">The new arguments of the method</param>
		/// <returns>A new <see cref="InvocationExpressionSyntax"/> with updated arguments</returns>
		public static InvocationExpressionSyntax WithArguments(this InvocationExpressionSyntax invocationExpression, params ExpressionSyntax[] arguments) {
			// wrap them in argument syntax first
			SeparatedSyntaxList<ArgumentSyntax> MappedArguments =
				SyntaxFactory.SeparatedList(arguments.Select(SyntaxFactory.Argument));
			return invocationExpression.WithArgumentList(invocationExpression.ArgumentList.WithArguments(MappedArguments));
		}

		/// <summary>
		///     Gets the return statement in a lambda. If the lambda is a block lambda, the return statement will only be returned
		///     if the block contains a single statement.
		/// </summary>
		/// <param name="lambdaExpression">The expression to extract the return expression from</param>
		/// <returns>The expression returned by the lambda, or null if the lambda is null or contains multiple statements</returns>
		public static CSharpSyntaxNode? GetSingleLambdaStatement(this LambdaExpressionSyntax lambdaExpression) {
			if (lambdaExpression.Body.Kind() != SyntaxKind.Block) return lambdaExpression.Body;

			// if it's a block, extract the return statement expr
			BlockSyntax Body = (BlockSyntax)lambdaExpression.Body;
			if (Body.Statements.Count != 1) return null;
			StatementSyntax LastStatement = Body.Statements[0];
			if (!(LastStatement is ReturnStatementSyntax ReturnStatement)) return null;
			return ReturnStatement.Expression;
		}

		/// <summary>
		///     Gets the a list of the parameters in a lambda expression
		/// </summary>
		/// <param name="lambdaExpression">The expression to extract the parameters from</param>
		/// <returns>The parameters taken in by the lambda</returns>
		public static IEnumerable<ParameterSyntax> GetParameters(this LambdaExpressionSyntax lambdaExpression) {
			IEnumerable<ParameterSyntax> Parameters = lambdaExpression switch {
				SimpleLambdaExpressionSyntax Simple => new[] { Simple.Parameter },
				ParenthesizedLambdaExpressionSyntax Many => Many.ParameterList.Parameters,
				_ => Enumerable.Empty<ParameterSyntax>()
			};
			return Parameters;
		}

		/// <summary>
		///    Changes the parameters of a lambda expression
		/// </summary>
		/// <param name="lambdaExpression">The expression to extract the parameters from</param>
		/// <param name="parameters">The new parameters to use with the lambda</param>
		/// <returns>A new lambda with updated parameters</returns>
		public static LambdaExpressionSyntax WithParameters(this LambdaExpressionSyntax lambdaExpression, params ParameterSyntax[] parameters) {
			CSharpSyntaxNode Body = lambdaExpression.Body;

			if (parameters.Length == 1) return SyntaxFactory.SimpleLambdaExpression(parameters[0], Body);
			return SyntaxFactory.ParenthesizedLambdaExpression(
				SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)),
				Body);
		}
	}
}