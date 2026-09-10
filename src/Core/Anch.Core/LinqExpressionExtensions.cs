using System.Linq.Expressions;
using System.Reflection;

namespace Anch.Core;

public static class LinqExpressionExtensions
{
    public static Expression<Func<TSourceArg, TSelectorResult>> Select<TSourceArg, TSourceResult, TSelectorResult>(
        this Expression<Func<TSourceArg, TSourceResult>> sourceExpr, Expression<Func<TSourceResult, TSelectorResult>> selector)
    {
        var freshParameter = Expression.Parameter(typeof(TSourceArg), sourceExpr.Parameters.Single().Name);

        var sourceBody = sourceExpr.Body.Override(sourceExpr.Parameters.Single(), freshParameter);

        return Expression.Lambda<Func<TSourceArg, TSelectorResult>>(
            selector.Body.Override(selector.Parameters.Single(), sourceBody),
            freshParameter);
    }

    public static Expression<Func<TSourceArg, TSelectorResult>> SelectMany<TSourceArg, TSourceResult, TNextResult, TSelectorResult>(
        this Expression<Func<TSourceArg, TSourceResult>> sourceExpr,
        Expression<Func<TSourceResult, Expression<Func<TSourceArg, TNextResult>>>> nextExpr,
        Expression<Func<TSourceResult, TNextResult, TSelectorResult>> resultSelector)
    {
        var freshParameter = Expression.Parameter(typeof(TSourceArg), sourceExpr.Parameters.Single().Name);

        var sourceExprWithFreshParameter = Expression.Lambda<Func<TSourceArg, TSourceResult>>(
            sourceExpr.Body.Override(sourceExpr.Parameters.Single(), freshParameter),
            freshParameter);

        var nextBody = VisitNextResult<TSourceArg, TSourceResult, TNextResult>(sourceExprWithFreshParameter, nextExpr.Body);

        return Expression.Lambda<Func<TSourceArg, TSelectorResult>>(

            resultSelector.Body.Override(resultSelector.Parameters[1], nextBody)
                .Override(resultSelector.Parameters[0], sourceExprWithFreshParameter.Body),

            freshParameter);
    }

    private static Expression VisitNextResult<TSourceArg, TSourceResult, TNextResult>(Expression<Func<TSourceArg, TSourceResult>> inputExpr,
        Expression nextExprBlock)
    {
        if (nextExprBlock is ConditionalExpression conditionalExpression)
        {
            return Expression.Condition(
                VisitNextTest(inputExpr, conditionalExpression.Test),
                VisitNextResult<TSourceArg, TSourceResult, TNextResult>(inputExpr, conditionalExpression.IfTrue),
                VisitNextResult<TSourceArg, TSourceResult, TNextResult>(inputExpr, conditionalExpression.IfFalse));
        }
        else if (nextExprBlock == inputExpr.Parameters.Single())
        {
            return inputExpr.Body;
        }
        else if (nextExprBlock is MemberExpression { Expression: ConstantExpression constantExpression } memberExpression)
        {
            var lambda = (LambdaExpression)((FieldInfo)memberExpression.Member).GetValue(constantExpression.Value)!;

            return lambda.Body.Override(lambda.Parameters.Single(), inputExpr.Parameters.Single());
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(nextExprBlock), "Unsupported expression type");
        }
    }

    private static Expression VisitNextTest<TSourceArg, TSourceResult>(Expression<Func<TSourceArg, TSourceResult>> sourceExpr, Expression nextExprBlock) =>
        nextExprBlock == sourceExpr.Parameters.Single() ? sourceExpr.Body : nextExprBlock;
}