using System.Collections.Immutable;

using Anch.Core;

namespace Anch.OData.Domain.QueryLanguage;

/// <summary>A call of one of the supported methods on a source expression.</summary>
/// <param name="Source">The object (or collection) the method is called on.</param>
/// <param name="Type">Which method is called.</param>
/// <param name="Arguments">The arguments; a lambda for <c>Any</c> and <c>All</c> with a condition.</param>
public record MethodExpression(Expression Source, MethodExpressionType Type, ImmutableArray<Expression?> Arguments) : Expression
{
    public override string ToString() => $"{this.Source}.{this.Type.ToFormatString()}({this.Arguments.Join(", ")})";

    public virtual bool Equals(MethodExpression? other) =>
        ReferenceEquals(this, other)
        || (other is not null
            && this.Type == other.Type
            && this.Source == other.Source
            && this.Arguments.SequenceEqual(other.Arguments));

    public override int GetHashCode() => base.GetHashCode() ^ this.Type.GetHashCode();
}
