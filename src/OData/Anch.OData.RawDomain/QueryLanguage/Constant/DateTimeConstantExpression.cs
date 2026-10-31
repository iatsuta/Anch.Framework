using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

/// <summary>A date and time constant.</summary>
/// <param name="Value">The value.</param>
public record DateTimeConstantExpression(DateTime Value) : ConstantExpression<DateTime>(Value);
