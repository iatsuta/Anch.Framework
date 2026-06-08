using System.Runtime.CompilerServices;

using NHibernate.Linq;

namespace Anch.GenericQueryable.NHibernate;

public static class NHibLinqExtensions
{
    public static async Task<TSource[]> ToArrayAsync<TSource>(
        IQueryable<TSource> source,
        CancellationToken ct = default) =>
        await AsAsyncEnumerable(source).ToArrayAsync(ct);

    public static async Task<HashSet<TSource>> ToHashSetAsync<TSource>(
        IQueryable<TSource> source,
        IEqualityComparer<TSource>? comparer,
        CancellationToken ct = default) =>
        await AsAsyncEnumerable(source).ToHashSetAsync(comparer, ct);

    public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
        IQueryable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        IEqualityComparer<TKey>? comparer,
        CancellationToken ct = default)
        where TKey : notnull =>
        await AsAsyncEnumerable(source).ToDictionaryAsync(keySelector, elementSelector, comparer, ct);

    public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(IQueryable<TSource> source) => source.AsAsyncEnumerableInternal();

    private static async IAsyncEnumerable<TSource> AsAsyncEnumerableInternal<TSource>(this IQueryable<TSource> source, [EnumeratorCancellation] CancellationToken ct = default)
    {
        foreach (var item in await source.ToFuture().GetEnumerableAsync(ct))
        {
            yield return item;
        }
    }
}