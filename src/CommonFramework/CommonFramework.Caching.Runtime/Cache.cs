using System.Collections.Concurrent;

namespace CommonFramework.Caching;

public class Cache<TKey, TValue>(object key) : ConcurrentDictionary<TKey, TValue>, ICache<TKey, TValue>
    where TKey : notnull
{
    public object Key { get; } = key;
}