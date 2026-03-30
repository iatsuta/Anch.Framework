using OData.QueryLanguage.Constant.Base;

namespace OData.QueryLanguage.Constant;

public record DecimalConstantExpression(decimal Value) : ConstantExpression<decimal>(Value);
