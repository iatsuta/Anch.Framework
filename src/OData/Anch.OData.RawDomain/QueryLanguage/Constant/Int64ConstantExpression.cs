using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

/// <summary>A 64-bit integer constant.</summary>
/// <param name="Value">The value.</param>
public record Int64ConstantExpression(long Value) : ConstantExpression<long>(Value);
