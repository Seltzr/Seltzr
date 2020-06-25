// -----------------------------------------------------------------------
// <copyright file="DelegateCondition.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace Seltzr.Conditions {
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using Seltzr.Context;

	/// <summary>
	///     Condition that uses a delegate to check if it has been met
	/// </summary>
	/// <typeparam name="TModel">The type of model in the dataset</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public class DelegateCondition<TModel, TUser> : ICondition<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The delegate to use to check the status of the condition
		/// </summary>
		private readonly Func<IApiContext<TModel, TUser>, IQueryable<TModel>, Task<bool>> ConditionDelegate;

		/// <summary>
		///     Initializes a new instance of the <see cref="DelegateCondition{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="conditionDelegate">The delegate to use to check the status of the condition</param>
		/// <param name="failureMessage">The message of the exception to throw when the condition is not met</param>
		public DelegateCondition(Func<IApiContext<TModel, TUser>, IQueryable<TModel>, Task<bool>> conditionDelegate, string? failureMessage = null) {
			this.ConditionDelegate = conditionDelegate;
			this.FailureMessage = failureMessage ?? "A required condition was not met";
		}

		/// <summary>
		///     Gets a message indicating why the condition might have failed
		/// </summary>
		public string FailureMessage { get; }

		/// <summary>
		///     Verifies that the current request meets a condition
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The current dataset to be filtered</param>
		/// <returns><see langword="true"/> if the request should continue, <see langword="false"/> otherwise</returns>
		public Task<bool> VerifyAsync(
			IApiContext<TModel, TUser> context,
			IQueryable<TModel> dataset) =>
			this.ConditionDelegate(context, dataset);
	}
}