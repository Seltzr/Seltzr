// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilderBase.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Options.Builder {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;


	/// <summary>
	///		Base class for <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> with useful static methods
	/// </summary>
	public class SeltzrOptionsBuilderBase {
		/// <summary>
		///     Extracts a property from an expression
		/// </summary>
		/// <param name="propertyExpression">The expression that refers to a property</param>
		/// <typeparam name="TProperty">The type of the property to extract</typeparam>
		/// <typeparam name="TModel">The model to extract the property from</typeparam>
		/// <returns>The extracted property</returns>
		protected static PropertyInfo ExtractProperty<TProperty, TModel>(Expression<Func<TModel, TProperty>> propertyExpression) {
			return (PropertyInfo)SeltzrOptionsBuilderBase.ExtractPropertyExpression(propertyExpression).Member;
		}

		/// <summary>
		///     Extracts the member access expression from an expression
		/// </summary>
		/// <param name="propertyExpression">The lambda expression that refers to a member access</param>
		/// <typeparam name="TProperty">The type of the property to extract</typeparam>
		/// <typeparam name="TModel">The model to extract the property from</typeparam>
		/// <returns>The extracted property access expression</returns>
		protected static MemberExpression ExtractPropertyExpression<TProperty, TModel>(Expression<Func<TModel, TProperty>> propertyExpression) {
			MemberExpression MemberExpression;
			if (propertyExpression.Body is UnaryExpression Body)
				MemberExpression = (MemberExpression)Body.Operand;
			else MemberExpression = (MemberExpression)propertyExpression.Body;
			return MemberExpression;
		}

		/// <summary>
		///     Fixes a route string provided through user input
		/// </summary>
		/// <param name="routeString">The inputted route string</param>
		/// <returns>The route string, ensuring that it ends with a / and starts with a letter</returns>
		protected static string FixRoute(string? routeString) {
			if (routeString == null) return string.Empty;
			if (routeString.StartsWith("/")) routeString = routeString.Substring(1);
			if (routeString.Length != 0 && !routeString.EndsWith("/")) routeString += "/";
			return routeString;
		}

		/// <summary>
		///		Gets a camelCased version of the C# property name
		/// </summary>
		/// <param name="property">The property to get the camelCased name of</param>
		/// <typeparam name="TParent">The type to which the property belongs</typeparam>
		/// <typeparam name="TProperty">The type of the property</typeparam>
		/// <returns>The camelCased name of <paramref name="property"/></returns>
		protected static string CamelCase<TParent, TProperty>(Expression<Func<TParent, TProperty>> property) => SeltzrOptionsBuilderBase.CamelCase(SeltzrOptionsBuilderBase.ExtractProperty(property));

		/// <summary>
		///		Gets a camelCased version of the C# property name
		/// </summary>
		/// <param name="property">The property to get the camelCased name of</param>
		/// <returns>The camelCased name of <paramref name="property"/></returns>
		protected static string CamelCase(PropertyInfo property) => SeltzrOptionsBuilderBase.CamelCase(property.Name);

		/// <summary>
		///		camelCases a string
		/// </summary>
		/// <param name="str">The string to camelCase</param>
		/// <returns>The string in camelCase format</returns>
		protected static string CamelCase(string str) {
			// little more complex to deal with ACRONYMCase --> acronymCase
			int LastUppercase = 0;
			while (LastUppercase < str.Length) {
				char Char = str[LastUppercase];
				if (Char >= 'a' && Char <= 'z')
					break;
				LastUppercase++;
			}

			LastUppercase--; // last uppercase is one before first lowercase

			if (LastUppercase < 1) LastUppercase = 1; // NotAnAcronym or alreadyCamelCase
			return str.Substring(0, LastUppercase).ToLower() + str.Substring(LastUppercase);
		}
	}
}