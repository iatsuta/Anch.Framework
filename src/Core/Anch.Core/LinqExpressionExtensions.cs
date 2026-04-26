using System.Linq.Expressions;
using System.Reflection;

namespace Anch.Core;

public static class LinqExpressionExtensions
{
    public static Expression<Func<TSourceArg, TSelectorResult>> Select<TSourceArg, TSourceResult, TSelectorResult>(this Expression<Func<TSourceArg, TSourceResult>> sourceExpr, Expression<Func<TSourceResult, TSelectorResult>> selector)
    {
        return Expression.Lambda<Func<TSourceArg, TSelectorResult>>(selector.Body.Override(selector.Parameters.Single(), sourceExpr.Body), sourceExpr.Parameters);
    }

    public static Expression<Func<TSourceArg, TSelectorResult>> SelectMany<TSourceArg, TSourceResult, TNextResult, TSelectorResult>(
        this Expression<Func<TSourceArg, TSourceResult>> sourceExpr,
        Expression<Func<TSourceResult, Expression<Func<TSourceArg, TNextResult>>>> nextExpr,
        Expression<Func<TSourceResult, TNextResult, TSelectorResult>> resultSelector)
    {
        var nextBody = VisitNextResult<TSourceArg, TSourceResult, TNextResult>(sourceExpr, nextExpr.Body);

        return Expression.Lambda<Func<TSourceArg, TSelectorResult>>(

            resultSelector.Body.Override(resultSelector.Parameters[1], nextBody)
                .Override(resultSelector.Parameters[0], sourceExpr.Body),

            sourceExpr.Parameters);
    }
    private static Expression VisitNextResult<TSourceArg, TSourceResult, TNextResult>(Expression<Func<TSourceArg, TSourceResult>> inputExpr, Expression nextExprBlock)
    {
        if (nextExprBlock is ConditionalExpression conditionalExpression)
        {
            return Expression.Condition(
                VisitNextTest(inputExpr, inputExpr),
                VisitNextResult<TSourceArg, TSourceResult, TNextResult>(inputExpr, conditionalExpression.IfTrue),
                VisitNextResult<TSourceArg, TSourceResult, TNextResult>(inputExpr, conditionalExpression.IfFalse));
        }

        if (nextExprBlock == inputExpr.Parameters.Single())
        {
            return inputExpr.Body;
        }

        if (nextExprBlock is MemberExpression { Expression: ConstantExpression constantExpression } memberExpression)
        {
            var lambda = (LambdaExpression)((FieldInfo)memberExpression.Member).GetValue(constantExpression.Value)!;

            return lambda.Body.Override(lambda.Parameters.Single(), inputExpr.Parameters.Single());
        }

        throw new NotImplementedException();
    }

    private static Expression VisitNextTest<TSourceArg, TSourceResult>(Expression<Func<TSourceArg, TSourceResult>> sourceExpr, Expression nextExprBlock)
    {
        return nextExprBlock == sourceExpr.Parameters.Single() ? sourceExpr.Body : nextExprBlock;
    }
}