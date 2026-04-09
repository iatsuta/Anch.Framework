using System.Collections.Immutable;

namespace CommonFramework;

public readonly record struct MergeResult<TSource, TTarget>(
    ImmutableArray<TTarget> AddingItems,
    ImmutableArray<ValueTuple<TSource, TTarget>> CombineItems,
    ImmutableArray<TSource> RemovingItems)
{
    public MergeResult(
        IEnumerable<TTarget> addingItems,
        IEnumerable<ValueTuple<TSource, TTarget>> combineItems,
        IEnumerable<TSource> removingItems)
        : this([..addingItems], [..combineItems], [..removingItems])
    {
    }

    public bool IsEmpty => !this.RemovingItems.Any() && !this.AddingItems.Any();

    public static readonly MergeResult<TSource, TTarget> Empty = new([], [], []);
}