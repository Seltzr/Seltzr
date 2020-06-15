using Seltzr.Operations;

using System.Collections.Generic;
using System.Linq;

namespace Seltzr.OrmBase {
	using System.Reflection;

	using Seltzr.Context;
	using Seltzr.ParameterRetrievers;

	public interface IOrmDataProvider<TModel, TUser> where TModel : class where TUser : class {
		IQueryable<TModel> GetModels(IApiContext<TModel, object> context);

		List<KeyProperty> GetPrimaryKey();

		IOperation<TModel, TUser> GetCreateOperation();

		IOperation<TModel, TUser> GetUpdateOperation(PropertyInfo[] properties, ParameterRetriever[]? retrievers);

		IOperation<TModel, TUser> GetDeleteOperation();
	}
}
