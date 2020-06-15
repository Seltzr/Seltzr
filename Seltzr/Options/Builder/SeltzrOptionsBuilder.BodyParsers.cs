// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilder.BodyParsers.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Options.Builder {
	using System.Text.Json;

	using Seltzr.Parsers;

	/// <summary>
	///     Builder for <see cref="SeltzrOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class SeltzrOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Sets this route up to parse JSON request bodies
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ParseJson() => this.ParseJson(null);

		/// <summary>
		///     Sets this route up to parse JSON request bodies
		/// </summary>
		/// <param name="options">The options for the JSON deserializer</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ParseJson(JsonSerializerOptions options) =>
			this.AddBodyParser(new JsonBodyParser<TModel>(options));

		/// <summary>
		///     Sets this route up to parse JSON request bodies as a single object or an array
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ParseJsonArrays() {
			this.AcceptArrays();
			return this.ParseJson();
		}

		/// <summary>
		///     Sets this route up to parse JSON request bodies as a single object or an array
		/// </summary>
		/// <param name="options">The options for the JSON deserializer</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ParseJsonArrays(JsonSerializerOptions options) {
			this.AcceptArrays();
			return this.ParseJson(options);
		}

		/// <summary>
		///     Sets this route up to parse XML request bodies
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ParseXml() => this.AddBodyParser<XmlBodyParser<TModel>>();

		/// <summary>
		///     Sets this route up to parse XML and JSON request bodies
		/// </summary>
		/// <param name="keepExisting">True to keep existing parsers, false otherwise</param>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ParseXmlAndJson(bool keepExisting = false) {
			if (!keepExisting) this.ClearBodyParsers();
			this.ParseJson();
			this.ParseXml();
			return this;
		}

		/// <summary>
		///     Sets this route up to parse XML and JSON request bodies as a single object or an array
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ParseXmlAndJsonArrays() {
			this.AcceptArrays();
			return this.ParseXmlAndJson();
		}

		/// <summary>
		///     Sets this route up to parse XML request bodies as a single object or an array
		/// </summary>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> ParseXmlArrays() {
			this.AcceptArrays();
			return this.ParseXml();
		}
	}
}