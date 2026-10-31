using System.Linq.Expressions;

namespace Anch.GenericQueryable.Services;

/// <summary>Rewrites a call to a <c>GenericXxx</c> operation into a call of its real implementation.</summary>
public interface IMethodRedirector
{
    /// <summary>Rewrites the call if this redirector supports it.</summary>
    /// <param name="callExpression">The call to rewrite.</param>
    /// <typeparam name="TResult">Result type of the call.</typeparam>
    /// <returns>The rewritten call, or <c>null</c> when not supported.</returns>
    Expression<Func<TResult>>? TryRedirect<TResult>(Expression<Func<TResult>> callExpression);
}