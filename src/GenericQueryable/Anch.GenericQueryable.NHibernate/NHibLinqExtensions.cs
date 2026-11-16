using System.Runtime.CompilerServices;

using NHibernate.Linq;

namespace Anch.GenericQueryable.NHibernate;

/// <summary>Async LINQ operators for NHibernate queries that NHibernate itself lacks; they run the query with <c>ToFuture</c>.</summary>
public static class NHibLinqExtensions
{
    /// <summary>Materializes the query into an array.</summary>
    /// <param name="source">The query.</param>
    /// <param name="ct">Cancellation token.</param>
    public static async Task<TSource[]> ToArrayAsync<TSource>(
        IQueryable<TSource> source,
        CancellationToken ct) =>
        await AsAsyncEnumerable(source).ToArrayAsync(ct);

    /// <summary>Materializes the query into a hash set.</summary>
    /// <param name="source">The query.</param>
    /// <param name="comparer">The comparer for the set; <c>null</c> for the default one.</param>
    /// <param name="ct">Cancellation token.</param>
    public static async Task<HashSet<TSource>> ToHashSetAsync<TSource>(
        IQueryable<TSource> source,
        IEqualityComparer<TSource>? comparer,
        CancellationToken ct) =>
        await AsAsyncEnumerable(source).ToHashSetAsync(comparer, ct);

    /// <summary>Materializes the query into a dictionary.</summary>
    /// <param name="source">The query.</param>
    /// <param name="keySelector">Selects the key.</param>
    /// <param name="elementSelector">Selects the value.</param>
    /// <param name="comparer">The key comparer; <c>null</c> for the default one.</param>
    /// <param name="ct">Cancellation token.</param>
    public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
        IQueryable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        IEqualityComparer<TKey>? comparer,
        CancellationToken ct)
        where TKey : notnull =>
        await AsAsyncEnumerable(source).ToDictionaryAsync(keySelector, elementSelector, comparer, ct);

    /// <summary>Exposes the query as an asynchronous sequence.</summary>
    /// <param name="source">The query.</param>
    public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(IQueryable<TSource> source) => source.AsAsyncEnumerableInternal(CancellationToken.None);

    private static async IAsyncEnumerable<TSource> AsAsyncEnumerableInternal<TSource>(this IQueryable<TSource> source, [EnumeratorCancellation] CancellationToken ct)
    {
        foreach (var item in await source.ToFuture().GetEnumerableAsync(ct))
        {
            yield return item;
        }
    }
}