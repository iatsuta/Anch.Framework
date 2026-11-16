using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

/// <summary>An evaluator that compiles every time without caching.</summary>
public class DefaultExpressionEvaluator : IExpressionEvaluator
{
    /// <inheritdoc/>
    public TDelegate Compile<TDelegate>(Expression<TDelegate> expression)
    {
        return expression.Compile();
    }
}