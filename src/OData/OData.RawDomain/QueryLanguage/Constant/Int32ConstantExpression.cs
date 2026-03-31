using OData.Domain.QueryLanguage.Constant.Base;

namespace OData.Domain.QueryLanguage.Constant;

public record Int32ConstantExpression(int Value) : ConstantExpression<int>(Value);
