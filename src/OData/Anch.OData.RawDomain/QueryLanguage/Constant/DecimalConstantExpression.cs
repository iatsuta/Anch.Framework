using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

/// <summary>A decimal constant.</summary>
/// <param name="Value">The value.</param>
public record DecimalConstantExpression(decimal Value) : ConstantExpression<decimal>(Value);
