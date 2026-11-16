using System.Collections.Immutable;

namespace Anch.OData.Domain;

/// <summary>One page of a query result together with the total number of matching items.</summary>
/// <param name="Items">The items of the page.</param>
/// <param name="TotalCount">The number of matching items without paging.</param>
/// <typeparam name="T">The item type.</typeparam>
public record SelectOperationResult<T>(ImmutableArray<T> Items, int TotalCount)
{
    /// <summary>Creates a result where the page is the whole result.</summary>
    /// <param name="items">The items.</param>
    public SelectOperationResult(ImmutableArray<T> items)
        : this(items, items.Length)
    {
    }

    /// <summary>Creates a result where the page is the whole result.</summary>
    /// <param name="items">The items.</param>
    public SelectOperationResult(IEnumerable<T> items)
        : this([.. items])
    {
    }

    /// <summary>Creates a result.</summary>
    /// <param name="items">The items of the page.</param>
    /// <param name="totalCount">The number of matching items without paging.</param>
    public SelectOperationResult(IEnumerable<T> items, int totalCount)
        : this([.. items], totalCount)
    {
    }
}
