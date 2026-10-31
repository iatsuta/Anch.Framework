using Anch.OData.Domain.QueryLanguage;

namespace Anch.OData.Domain;

/// <summary>One untyped <c>$orderby</c> clause.</summary>
/// <param name="Path">Selects the sort key.</param>
/// <param name="OrderType">The sort direction.</param>
public record SelectOrder(LambdaExpression Path, OrderType OrderType);
