using System.Linq.Expressions;

namespace Anch.GenericQueryable.Services;

public interface IMethodRedirector
{
	Expression<Func<TResult>>? TryRedirect<TResult>(Expression<Func<TResult>> callExpression);
}