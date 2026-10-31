using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

/// <summary>A GUID constant.</summary>
/// <param name="Value">The value.</param>
public record GuidConstantExpression(Guid Value) : ConstantExpression<Guid>(Value)
{
    public override string ToString() => $"'{this.Value}'";
}
