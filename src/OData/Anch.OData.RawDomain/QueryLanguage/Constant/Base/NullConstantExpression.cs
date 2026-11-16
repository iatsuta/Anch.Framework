namespace Anch.OData.Domain.QueryLanguage.Constant.Base;

/// <summary>The <c>null</c> constant.</summary>
public record NullConstantExpression : ConstantExpression
{
    public override object? GetRawValue() => null;

    public override string ToString() => "null";


    /// <summary>The only instance.</summary>
    public static readonly NullConstantExpression Value = new();
}
