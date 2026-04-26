namespace Anch.OData;

using SExpressions = System.Linq.Expressions;

public interface ILambdaExpressionConverter
{
    SExpressions.Expression<TDelegate> Convert<TDelegate>(OData.Domain.QueryLanguage.LambdaExpression expression);
}