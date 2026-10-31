namespace Anch.OData.Domain.QueryLanguage;

/// <summary>Access to a property of a source expression.</summary>
/// <param name="Source">The expression whose property is read.</param>
/// <param name="PropertyName">The property name.</param>
public record PropertyExpression(Expression Source, string PropertyName) : Expression
{
    public override string ToString() => $"{this.Source}.{this.PropertyName}";
}
