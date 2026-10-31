using Anch.OData.Domain.QueryLanguage;
using Anch.OData.Domain.QueryLanguage.Constant.Base;

using SExpressions = System.Linq.Expressions;

namespace Anch.OData;

/// <summary>The traversal of the untyped syntax tree: dispatches each node to the abstract conversion method for its kind.</summary>
public abstract class LambdaExpressionConverterBase : ILambdaExpressionConverter
{
    /// <summary>Converts a node.</summary>
    /// <param name="expression">The node.</param>
    /// <param name="parameters">The lambda parameters in scope.</param>
    /// <param name="expectedResultType">The expected delegate type, for lambdas; must be <c>null</c> for other nodes.</param>
    protected virtual SExpressions.Expression Convert(Expression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters, Type? expectedResultType = null)
    {
        if (expression is null) throw new ArgumentNullException(nameof(expression));
        if (parameters is null) throw new ArgumentNullException(nameof(parameters));

        if (expression is LambdaExpression lambdaExpression)
        {
            return this.Convert(lambdaExpression, parameters, expectedResultType);
        }

        if (expectedResultType is not null)
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


    /// <summary>Converts a lambda.</summary>
    /// <param name="expression">The lambda.</param>
    /// <param name="parameters">The lambda parameters in scope.</param>
    /// <param name="expectedResultType">The delegate type of the result.</param>
    protected abstract SExpressions.LambdaExpression Convert(LambdaExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters, Type? expectedResultType);

    /// <summary>Converts a binary operation.</summary>
    /// <param name="expression">The node.</param>
    /// <param name="parameters">The lambda parameters in scope.</param>
    protected abstract SExpressions.BinaryExpression Convert(BinaryExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters);

    /// <summary>Converts a method call.</summary>
    /// <param name="expression">The node.</param>
    /// <param name="parameters">The lambda parameters in scope.</param>
    protected abstract SExpressions.MethodCallExpression Convert(MethodExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters);

    /// <summary>Converts a parameter reference.</summary>
    /// <param name="expression">The node.</param>
    /// <param name="parameters">The lambda parameters in scope.</param>
    protected abstract SExpressions.ParameterExpression Convert(ParameterExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters);

    /// <summary>Converts a constant.</summary>
    /// <param name="expression">The node.</param>
    protected abstract SExpressions.ConstantExpression Convert(ConstantExpression expression);

    /// <summary>Converts a property access.</summary>
    /// <param name="expression">The node.</param>
    /// <param name="parameters">The lambda parameters in scope.</param>
    protected abstract SExpressions.MemberExpression Convert(PropertyExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters);

    /// <summary>Converts a unary operation.</summary>
    /// <param name="expression">The node.</param>
    /// <param name="parameters">The lambda parameters in scope.</param>
    protected abstract SExpressions.UnaryExpression Convert(UnaryExpression expression, Dictionary<ParameterExpression, SExpressions.ParameterExpression> parameters);

    /// <inheritdoc/>
    public SExpressions.Expression<TDelegate> Convert<TDelegate>(LambdaExpression expression) => (SExpressions.Expression<TDelegate>)this.Convert(expression, new Dictionary<ParameterExpression, SExpressions.ParameterExpression>(), typeof(TDelegate));
}
