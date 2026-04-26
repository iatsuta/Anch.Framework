using SExpressions = System.Linq.Expressions;

using Anch.OData.Domain.QueryLanguage.Operations;

namespace Anch.OData.Domain.QueryLanguage;

public record BinaryExpression(BinaryOperation Operation, Expression Left, Expression Right) : Expression
{
    public BinaryExpression(SExpressions.BinaryExpression binaryExpression)
        : this(binaryExpression.NodeType.ToBinaryOperation(), Create(binaryExpression.Left), Create(binaryExpression.Right))
    {
    }

    public override string ToString() => $"({this.Left} {this.Operation.ToFormatString()} {this.Right})";
}
