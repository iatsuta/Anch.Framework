using System.Collections.Immutable;

namespace Anch.OData.Domain;

public record SelectOperationResult<T>(ImmutableArray<T> Items, int TotalCount)
{
    public SelectOperationResult(ImmutableArray<T> items)
        : this(items, items.Length)
    {
    }

    public SelectOperationResult(IEnumerable<T> items)
        : this([..items])
    {
    }

    public SelectOperationResult(IEnumerable<T> items, int totalCount)
        : this([..items], totalCount)
    {
    }
}
