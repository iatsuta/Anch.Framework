using OData.QueryLanguage.Constant.Base;

namespace OData.QueryLanguage.Constant;

public record StringConstantExpression(string Value) : ConstantExpression<string>(Value)
{
    public override string ToString() => $"\"{this.Value}\"";
}
