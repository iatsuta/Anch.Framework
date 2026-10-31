using SExpressions = System.Linq.Expressions;

namespace Anch.OData.Domain.QueryLanguage.Operations;

/// <summary>Extensions for LINQ node types.</summary>
public static class ExpressionTypeExtensions
{
    /// <summary>Maps a LINQ binary node type to the query language operation.</summary>
    /// <param name="expressionType">The node type.</param>
    /// <exception cref="ArgumentOutOfRangeException">The node type is not supported.</exception>
    public static BinaryOperation ToBinaryOperation(this SExpressions.ExpressionType expressionType) =>
        expressionType switch
        {
            SExpressions.ExpressionType.Equal => BinaryOperation.Equal,
            SExpressions.ExpressionType.GreaterThanOrEqual => BinaryOperation.GreaterThanOrEqual,
            SExpressions.ExpressionType.LessThanOrEqual => BinaryOperation.LessThanOrEqual,
            SExpressions.ExpressionType.NotEqual => BinaryOperation.NotEqual,
            SExpressions.ExpressionType.GreaterThan => BinaryOperation.GreaterThan,
            SExpressions.ExpressionType.LessThan => BinaryOperation.LessThan,
            SExpressions.ExpressionType.Add => BinaryOperation.Add,
            SExpressions.ExpressionType.Subtract => BinaryOperation.Subtract,
            SExpressions.ExpressionType.OrElse => BinaryOperation.OrElse,
            SExpressions.ExpressionType.AndAlso => BinaryOperation.AndAlso,
            _ => throw new ArgumentOutOfRangeException(nameof(expressionType))
        };
}
