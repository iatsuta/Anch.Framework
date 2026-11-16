namespace Anch.OData.Domain.QueryLanguage;

/// <summary>A lambda parameter, referenced by name.</summary>
/// <param name="Name">The parameter name.</param>
public record ParameterExpression(string Name) : Expression
{
    public override string ToString() => this.Name;

    /// <summary>The default parameter, named <c>$arg</c>.</summary>
    public static ParameterExpression Default { get; } = new("$arg");
}
