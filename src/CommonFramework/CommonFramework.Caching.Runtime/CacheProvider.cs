using System.Collections.Concurrent;

namespace CommonFramework.Caching;

public class CacheProvider : ICacheProvider
{
    private readonly ConcurrentDictionary<object, object> cache = [];

    public ICache<TKey, TValue> GetCache<TKey, TValue>(object key)
        where TKey : notnull =>
        this.cache.GetOrAddAs(key, _ => new Cache<TKey, TValue>(key));
}