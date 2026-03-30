using OData.QueryLanguage.Constant.Base;

namespace OData.QueryLanguage.Constant;

public record GuidConstantExpression(Guid Value) : ConstantExpression<Guid>(Value)
{
    public override string ToString() => $"'{this.Value}'";
}
