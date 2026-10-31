using Anch.OData.Domain.QueryLanguage.Operations;

using SExpressions = System.Linq.Expressions;

namespace Anch.OData.Domain.QueryLanguage;

/// <summary>A binary operation: comparison, arithmetic or logical.</summary>
/// <param name="Operation">The operation.</param>
/// <param name="Left">The left operand.</param>
/// <param name="Right">The right operand.</param>
public record BinaryExpression(BinaryOperation Operation, Expression Left, Expression Right) : Expression
{
    /// <summary>Converts a LINQ binary expression.</summary>
    /// <param name="binaryExpression">The LINQ expression.</param>
    public BinaryExpression(SExpressions.BinaryExpression binaryExpression)
        : this(binaryExpression.NodeType.ToBinaryOperation(), Create(binaryExpression.Left), Create(binaryExpression.Right))
    {
    }

    public override string ToString() => $"({this.Left} {this.Operation.ToFormatString()} {this.Right})";
}
