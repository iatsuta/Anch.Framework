using System.Collections.ObjectModel;

namespace Anch.Core;

public static class EnumerableExtensions
{
    //public static FrozenDictionary<TKey, TValue> ToFrozenDictionary<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> pairs, IEqualityComparer<TKey>? comparer = null)
    //    where TKey : notnull =>
    //    pairs.ToFrozenDictionary(pair => pair.Item1, pair => pair.Item2, comparer);

    public static IEnumerable<T> CollectMaybe<T>(this IEnumerable<Maybe<T>> source) =>

        from item in source
        where item.HasValue
        select item.Value;

    public static IEnumerable<TState> Scan<TSource, TState>(this IEnumerable<TSource> source, TState state, Func<TState, TSource, TState> selector)
    {
        yield return state;

        foreach (var item in source)
        {
            yield return state = selector(state, item);
        }
    }

    public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source) => source.SelectMany(v => v);

    public static TResult Match<TSource, TResult>(this IEnumerable<TSource> source, Func<TResult> emptyFunc, Func<TSource, TResult> singleFunc,
        Func<TSource[], TResult> manyFunc)
    {
        var cache = source.ToArray();

        return cache.Length switch
        {
            0 => emptyFunc(),
            1 => singleFunc(cache.Single()),
            _ => manyFunc(cache)
        };
    }

    public static bool IsIntersected<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> other)
    {
        return source.IsIntersected(other, EqualityComparer<TSource>.Default);
    }

    public static bool IsIntersected<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> other, IEqualityComparer<TSource> comparer)
    {
        return source.Intersect(other, comparer).Any();
    }

    public static IEnumerable<TResult> EmptyIfNull<TResult>(this IEnumerable<TResult>? source)
    {
        return source ?? [];
    }

    public static IEnumerable<T> GetAllElements<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildFunc)
    {
        return source.SelectMany(child => child.GetAllElements(getChildFunc));
    }

    public static IEnumerable<T> GetAllElements<T>(this T source, Func<T, IEnumerable<T>> getChildFunc)
    {
        yield return source;

        foreach (var element in getChildFunc(source).SelectMany(child => child.GetAllElements(getChildFunc)))
        {
            yield return element;
        }
    }

    public static IEnumerable<T> GetAllElements<T>(this T? source, Func<T, T?> getNextFunc, bool skipFirstElement)
        where T : class
    {
        var baseElements = source.GetAllElements(getNextFunc);

        return skipFirstElement ? baseElements.Skip(1) : baseElements;
    }

    public static IEnumerable<T> GetAllElements<T>(this T? source, Func<T, T?> getNextFunc)
        where T : class
    {
        for (var state = source; state != null; state = getNextFunc(state))
        {
            yield return state;
        }
    }

    public static TResult GetByFirst<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource[], TResult> getMethod)
    {
        using var enumerator = source.GetEnumerator();

        if (enumerator.MoveNext())
        {
            return getMethod(enumerator.Current, enumerator.ReadToEnd().ToArray());
        }
        else
        {
            throw new Exception("Empty source");
        }
    }


    public static Maybe<TSource> SingleMaybe<TSource>(this IEnumerable<TSource> source)
    {
        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return Maybe<TSource>.Nothing;
        }

        var value = enumerator.Current;

        return Maybe.OfCondition(!enumerator.MoveNext(), () => value);
    }

    public static void Foreach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    public static void Foreach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        foreach (var pair in source.Select((value, index) => new { Value = value, Index = index }))
        {
            action(pair.Value, pair.Index);
        }
    }

    public static IReadOnlyList<T> ToReadOnlyListI<T>(this IEnumerable<T> source)
    {
        return source.ToList();
    }

    public static IEnumerable<TResult> ZipStrong<TSource, TOther, TResult>(this IEnumerable<TSource> source, IEnumerable<TOther> other,
        Func<TSource, TOther, TResult> resultSelector)
    {
        using var sourceEnumerator = source.GetEnumerator();
        using var otherEnumerator = other.GetEnumerator();

        while (true)
        {
            var res1 = sourceEnumerator.MoveNext();
            var res2 = otherEnumerator.MoveNext();

            if (res1 != res2)
            {
                throw new InvalidOperationException("The sequences had different lengths");
            }
            else if (res1)
            {
                yield return resultSelector(sourceEnumerator.Current, otherEnumerator.Current);
            }
            else
            {
                yield break;
            }
        }
    }

    public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
    {
        var hashSet = new HashSet<T>(comparer ?? EqualityComparer<T>.Default);

        return source.Where(item => !hashSet.Add(item));
    }

    public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<Exception> emptyExceptionHandler,
        Func<IReadOnlyCollection<TSource>, Exception> manyExceptionHandler)
    {
        return source.Match(
            () => throw emptyExceptionHandler(),
            value => value,
            items => throw manyExceptionHandler(items));
    }

    public static TSource? SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<Exception> manyExceptionHandler)
    {
        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return default;
        }

        var current = enumerator.Current;

        if (enumerator.MoveNext())
        {
            throw manyExceptionHandler();
        }

        return current;
    }

    public static TSource? SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate,
        Func<IReadOnlyCollection<TSource>, Exception> manyExceptionHandler)
    {
        return source.Where(predicate).SingleOrDefault(manyExceptionHandler);
    }

    public static TSource? SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<IReadOnlyCollection<TSource>, Exception> manyExceptionHandler)
    {
        var items = source.ToList();

        if (items.Count > 1)
        {
            throw manyExceptionHandler(items.ToArray());
        }

        return items.SingleOrDefault();
    }

    public static IReadOnlyCollection<T> ToReadOnlyCollectionI<T>(this IEnumerable<T> source)
    {
        return source.ToList();
    }

    public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
        where TKey : notnull
    {
        return new ReadOnlyDictionary<TKey, TValue>(source.ToDictionary(keySelector, elementSelector));
    }

    public static ReadOnlyDictionary<TKey, TSource> ToReadOnlyDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        where TKey : notnull
    {
        return source.ToReadOnlyDictionary(keySelector, v => v);
    }

    public static IReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionaryI<TValue, TKey>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        where TKey : notnull
    {
        return source.ToReadOnlyDictionary(keySelector);
    }

    public static IReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionaryI<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        where TKey : notnull
    {
        return source.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public static ReadOnlyCollection<TResult> ToReadOnlyCollection<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        return source.Select(selector).ToReadOnlyCollection();
    }

    public static ReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
    {
        return new ReadOnlyCollection<TSource>(source.ToArray());
    }

    public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<Exception> emptyExceptionHandler,
        Func<Exception> manyExceptionHandler)
    {
        return source.Where(predicate).Single(emptyExceptionHandler, manyExceptionHandler);
    }

    public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<Exception> emptyExceptionHandler)
    {
        return source.Single(predicate, emptyExceptionHandler, () => new InvalidOperationException("More Than One Element"));
    }

    public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<Exception> emptyExceptionHandler)
    {
        return source.Single(emptyExceptionHandler, () => new InvalidOperationException("More Than One Element"));
    }

    public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<Exception> emptyExceptionHandler, Func<Exception> manyExceptionHandler)
    {
        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            throw emptyExceptionHandler();
        }

        var current = enumerator.Current;

        if (enumerator.MoveNext())
        {
            throw manyExceptionHandler();
        }

        return current;
    }

    public static MergeResult<TSource, TTarget> GetMergeResult<TSource, TTarget, TKey>(
        this IEnumerable<TSource> source,
        IEnumerable<TTarget> target,
        Func<TSource, TKey> sourceKeySelector,
        Func<TTarget, TKey> targetKeySelector,
        Func<TKey, TKey, bool> equalsFunc)
        where TKey : notnull
    {
        return source.GetMergeResult(target, sourceKeySelector, targetKeySelector, new EqualityComparerImpl<TKey>(equalsFunc, _ => 0));
    }

    public static MergeResult<T, T> GetMergeResult<T>(this IEnumerable<T> source, IEnumerable<T> target, IEqualityComparer<T>? comparer = null)
        where T : notnull
    {
        return source.GetMergeResult(target, v => v, v => v, comparer);
    }

    public static MergeResult<TSource, TTarget> GetMergeResult<TSource, TTarget, TKey>(
        this IEnumerable<TSource> source,
        IEnumerable<TTarget> target,
        Func<TSource, TKey> sourceKeySelector,
        Func<TTarget, TKey> targetKeySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        var targetMap = target.ToDictionary(targetKeySelector, z => z, comparer ?? EqualityComparer<TKey>.Default);

        var removingItems = new List<TSource>();

        var combineItems = new List<ValueTuple<TSource, TTarget>>();

        foreach (var sourceItem in source)
        {
            var sourceKey = sourceKeySelector(sourceItem);

            if (targetMap.TryGetValue(sourceKey, out var targetItem))
            {
                combineItems.Add(ValueTuple.Create(sourceItem, targetItem));
                targetMap.Remove(sourceKey);
            }
            else
            {
                removingItems.Add(sourceItem);
            }
        }

        var addingItems = targetMap.Values.ToList();

        return new MergeResult<TSource, TTarget>(addingItems, combineItems, removingItems);
    }
}