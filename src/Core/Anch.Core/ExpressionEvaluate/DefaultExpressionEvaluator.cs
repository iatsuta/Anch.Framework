using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

public class DefaultExpressionEvaluator : IExpressionEvaluator
{
    public TDelegate Compile<TDelegate>(Expression<TDelegate> expression)
    {
        return expression.Compile();
    }
}