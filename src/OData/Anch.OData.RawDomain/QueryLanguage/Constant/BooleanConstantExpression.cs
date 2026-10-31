using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

/// <summary>A boolean constant.</summary>
/// <param name="Value">The value.</param>
public record BooleanConstantExpression(bool Value) : ConstantExpression<bool>(Value)
{
    /// <summary>The constant <c>true</c>.</summary>
    public static BooleanConstantExpression True { get; } = new(true);

    /// <summary>The constant <c>false</c>.</summary>
    public static BooleanConstantExpression False { get; } = new(false);
}