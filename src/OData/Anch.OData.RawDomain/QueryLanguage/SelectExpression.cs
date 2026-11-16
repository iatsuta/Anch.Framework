namespace Anch.OData.Domain.QueryLanguage;

/// <summary>A property access with an alias for the selected value.</summary>
/// <param name="Source">The expression whose property is read.</param>
/// <param name="PropertyName">The property name.</param>
/// <param name="Alias">The alias of the selected value.</param>
public record SelectExpression(Expression Source, string PropertyName, string Alias) : PropertyExpression(Source, PropertyName)
{
    public override string ToString() => $"{this.Source}.[{this.PropertyName} {this.Alias}]";
}
