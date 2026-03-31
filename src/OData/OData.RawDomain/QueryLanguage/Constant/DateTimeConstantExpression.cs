using OData.Domain.QueryLanguage.Constant.Base;

namespace OData.Domain.QueryLanguage.Constant;

public record DateTimeConstantExpression (DateTime Value) : ConstantExpression<DateTime>(Value);
