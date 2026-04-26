namespace Anch.OData.Domain.QueryLanguage;

public record PropertyExpression(Expression Source, string PropertyName) : Expression
{
    public override string ToString() => $"{this.Source}.{this.PropertyName}";
}
