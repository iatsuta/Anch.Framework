using Anch.Core;
using Anch.OData.Domain.QueryLanguage;

namespace Anch.OData.Domain;

/// <summary>Extensions for untyped OData lambdas.</summary>
public static class ExpressionExtensions
{
    /// <summary>Returns the chain of properties that a property-selecting lambda navigates, outermost first, each with its alias (if any).</summary>
    /// <param name="expression">A lambda whose body is a property chain starting at its parameter.</param>
    public static IEnumerable<Tuple<string, string?>> GetPropertyPath(this LambdaExpression expression)
    {
        if (expression is null) throw new ArgumentNullException(nameof(expression));

        var startParam = expression.Parameters.Single();

        var startProp = (PropertyExpression)expression.Body;

        var path = startProp.GetAllElements(prop => (prop.Source as ParameterExpression)
            .Maybe(s => s.Name == startParam.Name) ? null : (PropertyExpression)prop.Source);

        return path.Reverse().Select(expr => Tuple.Create(expr.PropertyName, (expr as SelectExpression).Maybe(selectExpr => selectExpr.Alias)));
    }
}
