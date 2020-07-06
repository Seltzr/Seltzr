// -----------------------------------------------------------------------
// <copyright file="IParseResult.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Parsers {
	using System.Reflection;

	/// <summary>
	///     The result of parsing a request body
	/// </summary>
	/// <typeparam name="TModel">The type of model being parsed</typeparam>
	public interface IParseResult<out TModel> {
		/// <summary>
		///     Gets the model that was parsed
		/// </summary>
		TModel ParsedModel { get; }

		/// <summary>
		///     Gets or sets a list of the properties that were present on this model in the request body
		/// </summary>
		PropertyInfo[] PresentProperties { get; set; }
	}
}