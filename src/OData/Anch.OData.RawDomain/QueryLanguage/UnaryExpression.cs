using Anch.OData.Domain.QueryLanguage.Operations;

namespace Anch.OData.Domain.QueryLanguage;

/// <summary>A unary operation, such as negation.</summary>
/// <param name="Operation">The operation.</param>
/// <param name="Operand">The operand.</param>
public record UnaryExpression(UnaryOperation Operation, Expression Operand) : Expression
{
    public override string ToString() => $"({this.Operation.ToFormatString()}{this.Operand})";

    public override int GetHashCode() => this.Operation.GetHashCode();

    public virtual bool Equals(UnaryExpression? other) =>
        ReferenceEquals(this, other)
        || (other is not null
            && this.Operation == other.Operation
            && this.Operand == other.Operand);
}
