using OData.Domain.QueryLanguage.Constant.Base;

namespace OData.Domain.QueryLanguage.Constant;

public record Int64ConstantExpression(long Value) : ConstantExpression<long>(Value);
