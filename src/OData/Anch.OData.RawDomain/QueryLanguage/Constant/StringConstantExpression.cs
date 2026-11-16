using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

/// <summary>A string constant.</summary>
/// <param name="Value">The value.</param>
public record StringConstantExpression(string Value) : ConstantExpression<string>(Value)
{
    public override string ToString() => $"\"{this.Value}\"";
}
