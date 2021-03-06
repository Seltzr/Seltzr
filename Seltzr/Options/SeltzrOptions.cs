﻿// -----------------------------------------------------------------------
// <copyright file="SeltzrOptions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Options {
	using System;
	using System.Collections.Generic;

	using Microsoft.AspNetCore.Builder;

	using Seltzr.Actions;
	using Seltzr.Auth;
	using Seltzr.Conditions;
	using Seltzr.ExceptionHandlers;
	using Seltzr.Filters;
	using Seltzr.Models;
	using Seltzr.Operations;
	using Seltzr.Parsers;
	using Seltzr.Responses;
	using Seltzr.Results;

	/// <summary>
	///     Options for a specific Seltzr route
	/// </summary>
	/// <typeparam name="TModel">The type of model this route handles</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public class SeltzrOptions<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Gets or sets a list of providers available to authenticate the request
		/// </summary>
		public List<IAuthProvider<TModel, TUser>>? AuthProviders { get; set; }

		/// <summary>
		///     Gets or sets the parsers available to parse a request body
		/// </summary>
		public List<IBodyParser<TModel>>? BodyParsers { get; set; }

		/// <summary>
		///     Gets or sets a list of conditions that the request must meet
		/// </summary>
		public List<ICondition<TModel, TUser>> Conditions { get; set; } = new List<ICondition<TModel, TUser>>();

		/// <summary>
		///     Gets or sets a list of filters to pare down the dataset
		/// </summary>
		public List<IFilter<TModel, TUser>> Filters { get; set; } = new List<IFilter<TModel, TUser>>();

		/// <summary>
		///     Gets or sets the provider for the API's models
		/// </summary>
		public IModelProvider<TModel, TUser>? ModelProvider { get; set; }

		/// <summary>
		///     Gets or sets the operation to perform on the dataset, if any
		/// </summary>
		public IOperation<TModel, TUser>? Operation { get; set; }

		/// <summary>
		///     Gets or sets the options for parsing request bodies
		/// </summary>
		public ParserOptions ParserOptions { get; set; } = new ParserOptions();

		/// <summary>
		///     Gets or sets the request methods these options match
		/// </summary>
		public HashSet<string>? RequestMethods { get; set; }

		/// <summary>
		///     Gets or sets the result writer for this route
		/// </summary>
		public IResultWriter<TModel, TUser>? ResultWriter { get; set; }

		/// <summary>
		///		Gets or sets any options for the <see cref="SeltzrOptions{TModel, TUser}.ResultWriter"/>
		/// </summary>
		public FormattingOptions FormattingOptions { get; set; } = new FormattingOptions();

		/// <summary>
		///     Gets or sets the pattern of the route that these options match with
		/// </summary>
		public string RoutePattern { get; set; } = "/";

		/// <summary>
		///		Gets or sets the list of exception handlers for this route
		/// </summary>
		public List<IExceptionHandler> ExceptionHandlers { get; set; } = new List<IExceptionHandler>();

		/// <summary>
		///		Gets or sets the list of pre-operation actions for this route
		/// </summary>
		public List<IPreOpAction<TModel, TUser>> PreOpActions { get; set; } = new List<IPreOpAction<TModel, TUser>>();

		/// <summary>
		///		Gets or sets the list of post-operation actions for this route
		/// </summary>
		public List<IPostOpAction<TModel, TUser>> PostOpActions { get; set; } = new List<IPostOpAction<TModel, TUser>>();

		/// <summary>
		///		Gets or sets the type of <see cref="Response{TModel}"/> to wrap responses in
		/// </summary>
		public Type? ResponseType { get; set; }

		/// <summary>
		///     Gets or sets the handler that will set route options, if any
		/// </summary>
		internal Action<IEndpointConventionBuilder>? RouteOptionsHandler { get; set; }

		/// <summary>
		///     Makes a copy of this options object and returns it
		/// </summary>
		/// <returns>The copy of this <see cref="SeltzrOptions{TModel, TUser}" /></returns>
		public SeltzrOptions<TModel, TUser> Copy() {
			return new SeltzrOptions<TModel, TUser> {
				                                           AuthProviders =
					                                           this.AuthProviders == null
						                                           ? null
						                                           : new List<IAuthProvider<TModel, TUser>>(
							                                           this.AuthProviders),
				                                           BodyParsers =
					                                           this.BodyParsers == null
						                                           ? null
						                                           : new List<IBodyParser<TModel>>(this.BodyParsers),
				                                           Conditions =
					                                           new List<ICondition<TModel, TUser>>(this.Conditions),
				                                           Filters = new List<IFilter<TModel, TUser>>(this.Filters),
				                                           ExceptionHandlers = new List<IExceptionHandler>(this.ExceptionHandlers),
														   PostOpActions = new List<IPostOpAction<TModel, TUser>>(this.PostOpActions),
														   PreOpActions = new List<IPreOpAction<TModel, TUser>>(this.PreOpActions),
				                                           FormattingOptions = this.FormattingOptions.Copy(),
				                                           ModelProvider = this.ModelProvider,
				                                           Operation = this.Operation,
				                                           ParserOptions = this.ParserOptions.Copy(),
				                                           RequestMethods = this.RequestMethods == null ? null : new HashSet<string>(this.RequestMethods),
				                                           ResultWriter = this.ResultWriter,
				                                           RouteOptionsHandler = this.RouteOptionsHandler,
				                                           RoutePattern = this.RoutePattern,
														   ResponseType = this.ResponseType
			                                           };
		}
	}
}