namespace Anch.OData;

using SExpressions = System.Linq.Expressions;

/// <summary>Converts an untyped OData lambda (a filter or an order key) into a LINQ expression.</summary>
public interface ILambdaExpressionConverter
{
    /// <summary>Converts the lambda.</summary>
    /// <param name="expression">The untyped lambda.</param>
    /// <typeparam name="TDelegate">The delegate type of the resulting expression.</typeparam>
    SExpressions.Expression<TDelegate> Convert<TDelegate>(Domain.QueryLanguage.LambdaExpression expression);
}