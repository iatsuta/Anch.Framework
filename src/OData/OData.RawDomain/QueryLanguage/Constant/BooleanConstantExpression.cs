using OData.Domain.QueryLanguage.Constant.Base;

namespace OData.Domain.QueryLanguage.Constant;

public record BooleanConstantExpression(bool Value) : ConstantExpression<bool>(Value)
{
    public static BooleanConstantExpression True { get; } = new(true);

    public static BooleanConstantExpression False { get; } = new(false);
}