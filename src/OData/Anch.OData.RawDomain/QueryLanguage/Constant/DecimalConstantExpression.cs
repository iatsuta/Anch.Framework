using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

public record DecimalConstantExpression(decimal Value) : ConstantExpression<decimal>(Value);
