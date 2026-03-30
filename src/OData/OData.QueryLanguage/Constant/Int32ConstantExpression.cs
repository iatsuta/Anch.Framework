using OData.QueryLanguage.Constant.Base;

namespace OData.QueryLanguage.Constant;

public record Int32ConstantExpression(int Value) : ConstantExpression<int>(Value);
