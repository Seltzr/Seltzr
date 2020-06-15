// -----------------------------------------------------------------------
// <copyright file="SeltzrOptionsBuilder.Template.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------
/*
namespace Seltzr.Options.Builder {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using Seltzr.Auth;
	using Seltzr.Conditions;
	using Seltzr.ExceptionHandlers;
	using Seltzr.Exceptions;
	using Seltzr.Filters;
	using Seltzr.Models;
	using Seltzr.Operations;
	using Seltzr.Parsers;
	using Seltzr.Results;

	/// <summary>
	///     Builder for <see cref="SeltzrOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class SeltzrOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="SeltzrOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public SeltzrOptionsBuilder<TModel, TUser> Template<TException>(Func<Exception, HttpContext, bool, Task<bool?>> handler) where TException : Exception {
			////this.Catch<TException>(new DelegateExceptionHandler(handler));
			return this;
		}
	}
}*/