namespace Anch.Core;

public readonly record struct MergeResult<TSource, TTarget>(
    IReadOnlyList<TTarget> AddingItems,
    IReadOnlyList<ValueTuple<TSource, TTarget>> CombineItems,
    IReadOnlyList<TSource> RemovingItems)
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