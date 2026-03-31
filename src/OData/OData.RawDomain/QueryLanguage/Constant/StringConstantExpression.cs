using OData.Domain.QueryLanguage.Constant.Base;

namespace OData.Domain.QueryLanguage.Constant;

public record StringConstantExpression(string Value) : ConstantExpression<string>(Value)
{
    public override string ToString() => $"\"{this.Value}\"";
}
