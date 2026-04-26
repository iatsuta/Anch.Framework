using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

public record DateTimeConstantExpression (DateTime Value) : ConstantExpression<DateTime>(Value);
