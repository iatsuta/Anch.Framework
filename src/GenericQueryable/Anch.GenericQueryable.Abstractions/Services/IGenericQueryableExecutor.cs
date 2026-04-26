using System.Linq.Expressions;
using Anch.GenericQueryable.Fetching;

namespace Anch.GenericQueryable.Services;

public interface IGenericQueryableExecutor
{
	IFetchService FetchService { get; }

    TResult Execute<TResult>(Expression<Func<TResult>> callExpression);
}