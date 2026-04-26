using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

public record GuidConstantExpression(Guid Value) : ConstantExpression<Guid>(Value)
{
    public override string ToString() => $"'{this.Value}'";
}
