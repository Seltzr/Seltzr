// -----------------------------------------------------------------------
// <copyright file="IResponse.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Responses {
	using System;
	using System.Reflection;

	using Seltzr.Responses.Attributes;

	/// <summary>
	///     An API response
	/// </summary>
	/// <typeparam name="TModel">The model that the response contains</typeparam>
	public interface IResponse<in TModel>
		where TModel : class {
		/// <summary>
		///		Gets all of the properties on this <see cref="IResponse{TModel}" /> that should be included in the serialized body
		/// </summary>
		/// <returns> All of the properties on this <see cref="IResponse{TModel}" /> that should be included in the serialized body</returns>
		PropertyInfo[] GetIncludedProperties();

		/// <summary>
		///		Gets all of the properties on this <see cref="IResponse{TModel}" /> that should not be included in the serialized body
		/// </summary>
		/// <returns> All of the properties on this <see cref="IResponse{TModel}" /> that should not be included in the serialized body</returns>
		PropertyInfo[] GetOmittedProperties();

		/// <summary>
		///		Sets the value of all properties on this <see cref="IResponse{TModel}"/> that have the given attribute applied to them. If no such properties exist, no action will occur.
		/// </summary>
		/// <typeparam name="TAttribute">The attribute to match on the properties to set</typeparam>
		/// <param name="value">The value to assign to that property</param>
		void Set<TAttribute>(object value) where TAttribute : Attribute;

		/// <summary>
		///		Sets the value of all properties on this <see cref="IResponse{TModel}"/> that have the <see cref="ResponseValueAttribute"/> with the given name applied to them. If no such properties exist, no action will occur.
		/// </summary>
		/// <param name="name">The name given to the <see cref="ResponseValueAttribute"/> on the properties to set</param>
		/// <param name="value">The value to assign to that property</param>
		void Set(string name, object value);

		/// <summary>
		///		Sets the value of all properties on this <see cref="IResponse{TModel}"/> that have the <see cref="ResponseValueAttribute"/> with the given name applied to them. If no such properties exist, no action will occur.
		/// </summary>
		/// <param name="name">The name given to the <see cref="ResponseValueAttribute"/> on the properties to set</param>
		/// <param name="value">The string value to assign to that property</param>
		/// <remarks>
		///		This method will attempt to convert the string value to a supported type if the type of the matching property is not string.
		/// </remarks>
		void SetString(string name, string value);

		/// <summary>
		///		Sets the value of all properties on this <see cref="IResponse{TModel}"/> that have the given attribute applied to them. If no such properties exist, no action will occur.
		/// </summary>
		/// <typeparam name="TAttribute">The attribute to match on the properties to set</typeparam>
		/// <param name="value">The string value to assign to that property</param>
		/// <remarks>
		///		This method will attempt to convert the string value to a supported type if the type of the matching property is not string.
		/// </remarks>
		void SetString<TAttribute>(string value) where TAttribute : Attribute;

		/// <summary>
		///		Populates the model property on this <see cref="IResponse{TModel}"/>
		/// </summary>
		/// <param name="models">The model dataset</param>
		/// <param name="shouldStrip"><see langword="true"/> to strip the array if there's only one element, <see langword="false"/> otherwise</param>
		void Populate(TModel[] models, bool shouldStrip);
	}
}