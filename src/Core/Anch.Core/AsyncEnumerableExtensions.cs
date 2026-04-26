using System.Collections.Immutable;

namespace Anch.Core;

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this ValueTask<T?> task)
    {
        var value = await task;

        if (value is not null)
        {
            yield return value;
        }
    }

    public static async ValueTask<ImmutableArray<T>> ToImmutableArrayAsync<T>(
        this IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        var builder = ImmutableArray.CreateBuilder<T>();

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            builder.Add(item);
        }

        return builder.ToImmutable();
    }

    public static async ValueTask<ImmutableList<T>> ToImmutableListAsync<T>(
        this IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        var builder = ImmutableList.CreateBuilder<T>();

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            builder.Add(item);
        }

        return builder.ToImmutable();
    }

    public static async ValueTask<ImmutableHashSet<T>> ToImmutableHashSetAsync<T>(
        this IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        var builder = ImmutableHashSet.CreateBuilder<T>();

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            builder.Add(item);
        }

        return builder.ToImmutable();
    }

    public static async ValueTask<ImmutableDictionary<TKey, TValue>> ToImmutableDictionaryAsync<TSource, TKey, TValue>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector,
        IEqualityComparer<TKey>? comparer,
        CancellationToken cancellationToken = default)
        where TKey : notnull
    {
        var builder = ImmutableDictionary.CreateBuilder<TKey, TValue>(comparer);

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            builder.Add(keySelector(item), valueSelector(item));
        }

        return builder.ToImmutable();
    }

    public static ValueTask<ImmutableDictionary<TKey, TValue>> ToImmutableDictionaryAsync<TSource, TKey, TValue>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector,
        CancellationToken cancellationToken = default)
        where TKey : notnull
        => source.ToImmutableDictionaryAsync(keySelector, valueSelector, comparer: null, cancellationToken);


    public static ValueTask<ImmutableDictionary<TKey, TSource>> ToImmutableDictionaryAsync<TSource, TKey>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        CancellationToken cancellationToken = default)
        where TKey : notnull
        => source.ToImmutableDictionaryAsync(keySelector, comparer: null, cancellationToken);

    public static ValueTask<ImmutableDictionary<TKey, TSource>> ToImmutableDictionaryAsync<TSource, TKey>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
        CancellationToken cancellationToken = default)
        where TKey : notnull
        => source.ToImmutableDictionaryAsync(keySelector, x => x, comparer, cancellationToken);


    public static ValueTask<ImmutableDictionary<TKey, TValue>> ToImmutableDictionaryAsync<TKey, TValue>(
        this IAsyncEnumerable<KeyValuePair<TKey, TValue>> source,
        CancellationToken cancellationToken = default)
        where TKey : notnull
        => source.ToImmutableDictionaryAsync(comparer: null, cancellationToken);

    public static ValueTask<ImmutableDictionary<TKey, TValue>> ToImmutableDictionaryAsync<TKey, TValue>(
        this IAsyncEnumerable<KeyValuePair<TKey, TValue>> source,
        IEqualityComparer<TKey>? comparer,
        CancellationToken cancellationToken = default)
        where TKey : notnull
        => source.ToImmutableDictionaryAsync(x => x.Key, x => x.Value, comparer, cancellationToken);


    public static IAsyncEnumerable<T> GetAllElements<T>(this IAsyncEnumerable<T> source, Func<T, IAsyncEnumerable<T>> getChildFunc)
    {
        return source.SelectMany(child => child.GetAllElements(getChildFunc));
    }

    public static IAsyncEnumerable<T> GetAllElements<T>(this T? source, Func<T, ValueTask<T?>> getNextFunc, bool skipFirstElement)
        where T : class
    {
        var baseElements = source.GetAllElements(getNextFunc);

        return skipFirstElement ? baseElements.Skip(1) : baseElements;
    }

    public static async IAsyncEnumerable<T> GetAllElements<T>(this T? source, Func<T, ValueTask<T?>> getNextFunc)
        where T : class
    {
        for (var state = source; state != null; state = await getNextFunc(state))
        {
            yield return state;
        }
    }

    public static async IAsyncEnumerable<T> GetAllElements<T>(this T source, Func<T, IAsyncEnumerable<T>> getChildFunc)
    {
        yield return source;

        await foreach (var element in getChildFunc(source).SelectMany(child => child.GetAllElements(getChildFunc)))
        {
            yield return element;
        }
    }
}