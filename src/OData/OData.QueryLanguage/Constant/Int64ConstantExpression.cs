using OData.QueryLanguage.Constant.Base;

namespace OData.QueryLanguage.Constant;

public record Int64ConstantExpression(long Value) : ConstantExpression<long>(Value);
