using Anch.OData.Domain.QueryLanguage;

namespace Anch.OData.Domain;

public record SelectOrder(LambdaExpression Path, OrderType OrderType);
