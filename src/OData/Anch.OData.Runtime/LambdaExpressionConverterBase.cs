using SExpressions = System.Linq.Expressions;

using Anch.OData.Domain.QueryLanguage;
using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData;

public abstract class LambdaExpressionConverterBase : ILambdaExpressionConverter
{
    protected virtual SExpressions.Expression Convert(Expression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters, Type? expectedResultType = null)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (parameters == null) throw new ArgumentNullException(nameof(parameters));

        if (expression is LambdaExpression lambdaExpression)
        {
            return this.Convert(lambdaExpression, parameters, expectedResultType);
        }

        if (expectedResultType != null)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedResultType));
        }

        return expression switch
        {
            BinaryExpression binaryExpression => this.Convert(binaryExpression, parameters),
            ParameterExpression parameterExpression => this.Convert(parameterExpression, parameters),
            ConstantExpression constantExpression => this.Convert(constantExpression),
            PropertyExpression propertyExpression => this.Convert(propertyExpression, parameters),
            UnaryExpression unaryExpression => this.Convert(unaryExpression, parameters),
            MethodExpression methodExpression => this.Convert(methodExpression, parameters),
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }


    protected abstract SExpressions.LambdaExpression Convert(LambdaExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters, Type? expectedResultType);

    protected abstract SExpressions.BinaryExpression Convert(BinaryExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters);

    protected abstract SExpressions.MethodCallExpression Convert(MethodExpression expression, Dictionary <ParameterExpression, SExpressions.ParameterExpression> parameters);

    protected abstract SExpressions.ParameterExpression Convert(ParameterExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters);

    protected abstract SExpressions.ConstantExpression Convert(ConstantExpression expression);

    protected abstract SExpressions.MemberExpression Convert(PropertyExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters);

    protected abstract SExpressions.UnaryExpression Convert(UnaryExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters);

    public SExpressions.Expression<TDelegate> Convert<TDelegate>(LambdaExpression expression) => (SExpressions.Expression<TDelegate>)this.Convert(expression, new Dictionary<ParameterExpression, SExpressions.ParameterExpression>(), typeof(TDelegate));
}
