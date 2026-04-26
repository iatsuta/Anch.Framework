using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

public static class ExpressionEvaluateHelper
{
    private static readonly IExpressionEvaluator ExpressionEvaluator = new FakeExpressionEvaluator();

    public static Expression<T> InlineEvaluate<T>(Func<IExpressionEvaluator, Expression<T>> func)
    {
        return func(ExpressionEvaluator).ExpandConst().InlineEvaluate();
    }
}