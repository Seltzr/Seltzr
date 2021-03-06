﻿// -----------------------------------------------------------------------
// <copyright file="IBodyParser.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Parsers {
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using Seltzr.Options;
	
	/// <summary>
	///     Parser for a request body
	/// </summary>
	/// <typeparam name="TModel">The type to parse to</typeparam>
	public interface IBodyParser<TModel> {
		/// <summary>
		///     Parses a request body
		/// </summary>
		/// <param name="body">The data of the request body</param>
		/// <param name="options">Options for the parser</param>
		/// <param name="context">The context for the HTTP request</param>
		/// <returns>The parsed models</returns>
		Task<ParseResult<TModel>[]> Parse(byte[] body, ParserOptions options, HttpContext context);
		
		// todo: should we add a Response<TModel> parameter?

		/// <summary>
		///		Gets whether or not the request body can be parsed by this <see cref="IBodyParser{TModel}"/>
		/// </summary>
		/// <param name="context">The context for the HTTP request</param>
		/// <returns><see langword="true"/> if the request body can be parsed by this <see cref="IBodyParser{TModel}"/>, <see langword="false"/> otherwise</returns>
		Task<bool> CanParse(HttpContext context);
	}
}