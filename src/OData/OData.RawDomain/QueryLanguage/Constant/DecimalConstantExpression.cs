using OData.Domain.QueryLanguage.Constant.Base;

namespace OData.Domain.QueryLanguage.Constant;

public record DecimalConstantExpression(decimal Value) : ConstantExpression<decimal>(Value);
