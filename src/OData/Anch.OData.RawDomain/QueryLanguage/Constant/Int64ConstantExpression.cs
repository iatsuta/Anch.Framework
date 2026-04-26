using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

public record Int64ConstantExpression(long Value) : ConstantExpression<long>(Value);
