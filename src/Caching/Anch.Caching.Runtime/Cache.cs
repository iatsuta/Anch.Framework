using System.Collections.Concurrent;

namespace Anch.Caching;

public class Cache<TKey, TValue>(object rootKey) : ConcurrentDictionary<TKey, TValue>, ICache<TKey, TValue>
    where TKey : notnull
{
    public object RootKey { get; } = rootKey;
}