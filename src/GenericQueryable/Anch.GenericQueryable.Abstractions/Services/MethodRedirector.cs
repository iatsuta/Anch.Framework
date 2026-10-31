using System.Linq.Expressions;
using System.Reflection;

namespace Anch.GenericQueryable.Services;

/// <summary>
/// Base redirector: replaces a call to a <c>GenericXxx</c> method with a call to the target method found by an <see cref="ITargetMethodExtractor"/>,
/// keeping the arguments the target method accepts (the trailing <see cref="CancellationToken"/> is dropped when the target has no such parameter).
/// </summary>
/// <param name="targetMethodExtractor">Finds the target method for a <c>GenericXxx</c> method.</param>
public class MethodRedirector(ITargetMethodExtractor targetMethodExtractor) : IMethodRedirector
{
    /// <inheritdoc/>
    public Expression<Func<TResult>>? TryRedirect<TResult>(Expression<Func<TResult>> callExpression)
    {
        if (callExpression.Body is MethodCallExpression methodCallExpression)
        {
            var targetMethod = targetMethodExtractor.TryGetTargetMethod(methodCallExpression.Method);

            if (targetMethod is not null)
            {
                var args = methodCallExpression.Arguments.Take(targetMethod.GetParameters().Length);

                var newCallExpression = this.CreateCallExpression(targetMethod, args);

                return Expression.Lambda<Func<TResult>>(newCallExpression);
            }
        }

        return null;
    }

    /// <summary>Builds the call of the target method; override to adapt the arguments or the result.</summary>
    /// <param name="targetMethod">The method to call.</param>
    /// <param name="args">The arguments of the original call that the target method accepts.</param>
    protected virtual Expression CreateCallExpression(MethodInfo targetMethod, IEnumerable<Expression> args)
    {
        return Expression.Call(targetMethod, args);
    }
}