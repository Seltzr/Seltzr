// -----------------------------------------------------------------------
// <copyright file="ModelCountAttribute.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Responses.Attributes {
	using System;

	/// <summary>
	///     Attribute that indicates that the property is a response value that should be set to the number of models returned
	///     in the request
	///     using pagination
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ModelCountAttribute : Attribute { }
}