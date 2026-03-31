using CommonFramework;

using OData.Domain.QueryLanguage;
using OData.Domain.QueryLanguage.Constant.Base;
using OData.Domain.QueryLanguage.Operations;

using SExpressions = System.Linq.Expressions;

namespace OData;

public class LambdaExpressionConverter : LambdaExpressionConverterBase
{
    protected override SExpressions.LambdaExpression Convert(LambdaExpression expression,
        Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters, Type? expectedResultType)
    {
        var parameterPairs = expression.Parameters.ZipStrong(expectedResultType!.GetGenericArguments().SkipLast(1), (parameter, type) =>
            new { InParameter = parameter, OutParameter = SExpressions.Expression.Parameter(type, parameter.Name) }).ToList();

        var sParameters = parameterPairs.ToDictionary(pair => pair.InParameter, pair => pair.OutParameter);

        var sumParameters = sParameters.Concat(parameters);

        var bodyExpr = this.Convert(expression.Body, sumParameters);

        return SExpressions.Expression.Lambda(expectedResultType, bodyExpr, parameterPairs.Select(pair => pair.OutParameter));
    }

    protected override SExpressions.BinaryExpression Convert(BinaryExpression expression,
        Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters)
    {
        var preLeftStandardExpression = this.Convert(expression.Left, parameters);
        var preRightStandardExpression = this.Convert(expression.Right, parameters);

        var leftStandardExpression = preLeftStandardExpression.TryNormalize(preLeftStandardExpression.Type, preRightStandardExpression.Type);

        var rightStandardExpression = preRightStandardExpression.TryNormalize(preLeftStandardExpression.Type, preRightStandardExpression.Type);


        return SExpressions.Expression.MakeBinary(expression.Operation.ToExpressionType(), leftStandardExpression, rightStandardExpression);
    }


    protected override SExpressions.MethodCallExpression Convert(MethodExpression expression,
        Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters)
    {
        var standardSourceExpr = this.Convert(expression.Source, parameters);

        var elementType = standardSourceExpr.Type.GetCollectionElementType()!;

        var delegateType = elementType.Maybe(v => typeof(Func<,>).MakeGenericType(v, typeof(bool)));

        var standardArguments = expression.Arguments.Select(arg => this.Convert(arg!, parameters, delegateType)).ToList();

        var allArgs = new[] { standardSourceExpr }.Concat(standardArguments).ToList();

        return expression.Type switch
        {
            MethodExpressionType.StringStartsWith => SExpressions.Expression.Call(
                TryCallToString(standardSourceExpr),
                MethodInfoHelper.StringStartsWithMethod,
                standardArguments),
            MethodExpressionType.StringContains => SExpressions.Expression.Call(TryCallToString(standardSourceExpr), MethodInfoHelper.StringContainsMethod,
                standardArguments),
            MethodExpressionType.StringEndsWith => SExpressions.Expression.Call(TryCallToString(standardSourceExpr), MethodInfoHelper.StringEndsWithMethod,
                standardArguments),
            MethodExpressionType.CollectionAny => SExpressions.Expression.Call(
                standardArguments.Any()
                    ? MethodInfoHelper.CollectionAnyFilterMethod.MakeGenericMethod(elementType)
                    : MethodInfoHelper.CollectionAnyEmptyMethod.MakeGenericMethod(elementType),
                allArgs),
            MethodExpressionType.CollectionAll => SExpressions.Expression.Call(MethodInfoHelper.CollectionAllFilterMethod.MakeGenericMethod(elementType),
                allArgs),
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }

    protected override SExpressions.ParameterExpression Convert(ParameterExpression expression,
        Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters) => parameters[expression];

    protected override SExpressions.ConstantExpression Convert(ConstantExpression expression) => SExpressions.Expression.Constant(expression.GetRawValue());

    protected override SExpressions.MemberExpression Convert(PropertyExpression expression,
        Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters)
    {
        var preSourceExpr = this.Convert(expression.Source, parameters);

        var sourceExpr = preSourceExpr.Type.IsNullable() ? SExpressions.Expression.Property(preSourceExpr, "Value") : preSourceExpr;

        return ExpressionHelper.PropertyOrFieldAuto(sourceExpr, expression.PropertyName);
    }

    protected override SExpressions.UnaryExpression Convert(UnaryExpression expression,
        Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters)
    {
        var standardOperand = this.Convert(expression.Operand, parameters);

        return SExpressions.Expression.MakeUnary(expression.Operation.ToExpressionType(), standardOperand, standardOperand.Type);
    }

    private static SExpressions.Expression TryCallToString(SExpressions.Expression source)
    {
        if (source is SExpressions.MemberExpression memberExpression && memberExpression.Type != typeof(string))
        {
            return SExpressions.Expression.Call(memberExpression, nameof(string.ToString), []);
        }
        else
        {
            return source;
        }
    }
}