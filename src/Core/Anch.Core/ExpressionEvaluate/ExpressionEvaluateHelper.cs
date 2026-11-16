using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

/// <summary>Helpers for writing expressions that call other expressions.</summary>
public static class ExpressionEvaluateHelper
{
    private static readonly IExpressionEvaluator ExpressionEvaluator = new FakeExpressionEvaluator();

    /// <summary>Builds an expression in which calls to <c>Evaluate</c> on the supplied evaluator are replaced with the bodies of the evaluated expressions, so the result can be translated by an ORM.</summary>
    /// <param name="func">Builds the expression using the evaluator.</param>
    /// <typeparam name="T">The delegate type of the expression.</typeparam>
    public static Expression<T> InlineEvaluate<T>(Func<IExpressionEvaluator, Expression<T>> func)
    {
        return func(ExpressionEvaluator).ExpandConst().InlineEvaluate();
    }
}