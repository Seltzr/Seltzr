// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilderExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace - keep it all together
namespace Seltzr.Extensions {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	using Seltzr.Exceptions;
	using Seltzr.Options.Builder;
	using Seltzr.OrmBase;
	using Seltzr.OrmBase.Filters;
	using Seltzr.OrmBase.Options;
	using Seltzr.ParameterRetrievers;

	/// <summary>
	///     Extension methods for the <see cref="SeltzrOptionsBuilder{TModel,TUser}" /> class
	/// </summary>
	public static partial class SeltzrOptionsBuilderExtensions {
		/// <summary>
		///     Ensures that the primary key of the <typeparamref name="TModel" /> will not be parsed from the request body
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> IgnorePrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			foreach (KeyProperty Property in SeltzrOptionsBuilderExtensions.GetPrimaryKey(builder))
				builder.Ignore(Property.PropertyInfo);
			return builder;
		}

		/// <summary>
		///     Ensures that the primary key of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> IncludePrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			foreach (KeyProperty Property in SeltzrOptionsBuilderExtensions.GetPrimaryKey(builder))
				builder.Include(Property.PropertyInfo);
			return builder;
		}


		/// <summary>
		///     Ensures that the primary key of the <typeparamref name="TModel" /> will be omitted from the response body
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> OmitPrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			foreach (KeyProperty Property in SeltzrOptionsBuilderExtensions.GetPrimaryKey(builder))
				builder.Omit(Property.PropertyInfo);
			return builder;
		}


		/// <summary>
		///     Filter's this route's dataset by a request parameter that is compared to the primary key of the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameters">The parameters to filter by</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> FilterByPrimaryKey<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			params ParameterRetriever[] parameters)
			where TModel : class where TUser : class {
			return builder.AddFilter(new PrimaryKeyFilter<TModel>(SeltzrOptionsBuilderExtensions.GetPrimaryKey(builder), parameters));
		}

		/// <summary>
		///     Filter's this route's dataset by query parameters compared to the primary key of the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterNames">The names of the query parameters to filter by</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> FilterByPrimaryKeyQuery<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			params string[] parameterNames)
			where TModel : class where TUser : class =>
			SeltzrOptionsBuilderExtensions.FilterByPrimaryKey<TModel, TUser, QueryParameterRetriever>(
				builder,
				parameterNames);

		/// <summary>
		///     Filter's this route's dataset by query parameters compared to the primary key of the model. The query parameters'
		///     names will be camelCased versions of their C# property name
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> FilterByPrimaryKeyQuery<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class =>
			builder.FilterByPrimaryKeyQuery(SeltzrOptionsBuilderExtensions.GetKeyNames(builder));

		/// <summary>
		///     Filter's this route's dataset by route values compared to the primary key of the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterNames">The names of the query parameters to filter by</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> FilterByPrimaryKeyRoute<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder,
			params string[] parameterNames)
			where TModel : class where TUser : class =>
			SeltzrOptionsBuilderExtensions.FilterByPrimaryKey<TModel, TUser, RouteValueRetriever>(
				builder,
				parameterNames);

		/// <summary>
		///     Filter's this route's dataset by route values compared to the primary key of the model. The query parameters' names
		///     will be camelCased versions of their C# property name
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static SeltzrOptionsBuilder<TModel, TUser> FilterByPrimaryKeyRoute<TModel, TUser>(
			this SeltzrOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class =>
			builder.FilterByPrimaryKeyRoute(SeltzrOptionsBuilderExtensions.GetKeyNames(builder));

		/// <summary>
		///     Filter's this route's dataset by a query parameter that is compared to the primary key of the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <typeparam name="TRetriever">The type of parameter retriever to use</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterNames">The names of the query parameters to filter by</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		private static SeltzrOptionsBuilder<TModel, TUser> FilterByPrimaryKey<TModel, TUser, TRetriever>(
			SeltzrOptionsBuilder<TModel, TUser> builder,
			params string[] parameterNames)
			where TModel : class where TUser : class where TRetriever : ParameterRetriever {
			IEnumerable<ParameterRetriever> ParameterRetrievers = parameterNames.Select(
				n => (ParameterRetriever)Activator.CreateInstance(typeof(TRetriever), n, null)!);
			return builder.FilterByPrimaryKey(ParameterRetrievers.ToArray());
		}

		/// <summary>
		///		Gets camelCased versions of the names of the primary key properties
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The builder containing the type of database context to get the keys for</param>
		/// <returns>camelCased versions of the primary key property names, in order</returns>
		private static string[] GetKeyNames<TModel, TUser>(SeltzrOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			List<KeyProperty> KeyProperties = SeltzrOptionsBuilderExtensions.GetPrimaryKey(builder);

			IEnumerable<string> Names =
				KeyProperties.Select(p => SeltzrOptionsBuilderExtensions.CamelCase(p.Name));
			return Names.ToArray();
		}

		/// <summary>
		///		camelCases a string
		/// </summary>
		/// <param name="str">The string to camelCase</param>
		/// <returns>The string in camelCase format</returns>
		private static string CamelCase(string str) {
			// todo: Expose CamelCase in regular Seltzr and use that instead
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

		/// <summary>
		///		Gets the properties that make up the primary key for the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The builder containing the type of database context to get the keys for</param>
		/// <returns>The properties that make up the primary key, in order</returns>
		private static List<KeyProperty> GetPrimaryKey<TModel, TUser>(SeltzrOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			if (!(builder is IOrmSeltzrOptionsBuilder<TModel, TUser> OrmBuilder))
				throw new OptionsException(
					"GetPrimaryKey can only be called on a builder that inherits from IOrmSeltzrOptionsBuilder");

			return OrmBuilder.GetPrimaryKey();
		}


		/// <summary>
		///     Extracts a property from an expression
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TProperty">The type of the property to extract</typeparam>
		/// <param name="propertyExpression">The expression that refers to a property</param>
		/// <returns>The extracted property</returns>
		private static PropertyInfo ExtractProperty<TModel, TProperty>(
			Expression<Func<TModel, TProperty>> propertyExpression)
			where TModel : class {
			MemberExpression MemberExpression;
			if (propertyExpression.Body is UnaryExpression Body)
				MemberExpression = (MemberExpression)Body.Operand;
			else MemberExpression = (MemberExpression)propertyExpression.Body;

			return (PropertyInfo)MemberExpression.Member;
		}
	}
}