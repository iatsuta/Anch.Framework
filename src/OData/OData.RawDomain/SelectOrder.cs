using OData.Domain.QueryLanguage;

namespace OData.Domain;

public record SelectOrder(LambdaExpression Path, OrderType OrderType);
