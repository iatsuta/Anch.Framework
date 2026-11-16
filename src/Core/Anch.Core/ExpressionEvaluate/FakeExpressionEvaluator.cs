using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

/// <summary>A placeholder evaluator that is never run: expressions that call it are rewritten by <c>InlineEvaluate</c>.</summary>
public class FakeExpressionEvaluator : IExpressionEvaluator
{
    /// <inheritdoc/>
    public TDelegate Compile<TDelegate>(Expression<TDelegate> expression)
    {
        throw new NotImplementedException();
    }
}