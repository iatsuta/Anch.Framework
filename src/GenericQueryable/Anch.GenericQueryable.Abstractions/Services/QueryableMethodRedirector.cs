using System.Linq.Expressions;
using System.Reflection;

using Anch.Core;

namespace Anch.GenericQueryable.Services;

/// <summary>
/// Redirects <c>GenericXxxAsync</c> calls to the synchronous <see cref="System.Linq.Queryable"/> operators and wraps the result in a completed <see cref="Task{TResult}"/>.
/// Used for in-memory queryables.
/// </summary>
/// <param name="targetMethodExtractor">Finds the <see cref="System.Linq.Queryable"/> operator for a <c>GenericXxxAsync</c> method.</param>
public class QueryableMethodRedirector(ITargetMethodExtractor targetMethodExtractor) : MethodRedirector(targetMethodExtractor)
{
    private static readonly MethodInfo TaskFromResultMethod = typeof(Task).GetMethod(nameof(Task.FromResult))!;

    /// <inheritdoc/>
    protected override Expression CreateCallExpression(MethodInfo targetMethod, IEnumerable<Expression> args)
    {
        return base.CreateCallExpression(targetMethod, args).Pipe(WrapToAsync);
    }

    private static Expression WrapToAsync(Expression callExpression)
    {
        return Expression.Call(TaskFromResultMethod.MakeGenericMethod(callExpression.Type), callExpression);
    }

    /// <summary>Redirector over <see cref="System.Linq.Queryable"/> operators.</summary>
    public static IMethodRedirector Default { get; } = new QueryableMethodRedirector(QueryableTargetMethodExtractor.Default);
}