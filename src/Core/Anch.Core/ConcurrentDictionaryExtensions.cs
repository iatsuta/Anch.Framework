using System.Collections.Concurrent;

namespace Anch.Core;

public static class ConcurrentDictionaryExtensions
{
    public static TNestedValue GetOrAddAs<TKey, TValue, TNestedValue>(
        this ConcurrentDictionary<TKey, TValue> concurrentDictionary, TKey key,
        Func<TKey, TNestedValue> valueFactory)
        where TKey : notnull
        where TValue : class
        where TNestedValue : TValue
        => concurrentDictionary.GetOrAdd(key, v => valueFactory(v)).Pipe(v => (TNestedValue)v);
}