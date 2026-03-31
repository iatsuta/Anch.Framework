using System.Collections.Immutable;
using OData.Domain.QueryLanguage;

namespace OData.Domain;

public interface IDynamicSelectOperation
{
    ImmutableArray<LambdaExpression> Expands { get; }

    ImmutableArray<LambdaExpression> Selects { get; }
}
