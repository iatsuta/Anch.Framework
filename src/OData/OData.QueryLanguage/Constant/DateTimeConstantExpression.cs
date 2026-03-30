using OData.QueryLanguage.Constant.Base;

namespace OData.QueryLanguage.Constant;

public record DateTimeConstantExpression (DateTime Value) : ConstantExpression<DateTime>(Value);
