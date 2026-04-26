using System.Collections.Immutable;

using Anch.OData.Domain.QueryLanguage;

namespace Anch.OData.Domain;

public interface IDynamicSelectOperation
{
    ImmutableArray<LambdaExpression> Expands { get; }

    ImmutableArray<LambdaExpression> Selects { get; }
}
