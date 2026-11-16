namespace Anch.Core;

/// <summary>The outcome of merging two collections: what to add, what to update and what to remove.</summary>
/// <param name="AddingItems">Target items that have no counterpart in the source.</param>
/// <param name="CombineItems">Pairs of a source item and its matching target item.</param>
/// <param name="RemovingItems">Source items that have no counterpart in the target.</param>
/// <typeparam name="TSource">The type of the existing items.</typeparam>
/// <typeparam name="TTarget">The type of the desired items.</typeparam>
public readonly record struct MergeResult<TSource, TTarget>(
    IReadOnlyList<TTarget> AddingItems,
    IReadOnlyList<ValueTuple<TSource, TTarget>> CombineItems,
    IReadOnlyList<TSource> RemovingItems)
{
    public MergeResult(
        IEnumerable<TTarget> addingItems,
        IEnumerable<ValueTuple<TSource, TTarget>> combineItems,
        IEnumerable<TSource> removingItems)
        : this([.. addingItems], [.. combineItems], [.. removingItems])
    {
    }

    /// <summary><c>true</c> if there is nothing to add or remove (matching items may still be updated).</summary>
    public bool IsEmpty => !this.RemovingItems.Any() && !this.AddingItems.Any();

    /// <summary>A result with no changes.</summary>
    public static readonly MergeResult<TSource, TTarget> Empty = new([], [], []);
}