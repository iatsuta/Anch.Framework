using System.Collections.Concurrent;

using Anch.Core;

namespace Anch.Caching;

public class CacheProvider : ICacheProvider
{
    private readonly ConcurrentDictionary<object, ICache> cache = [];

    public ICache<TKey, TValue> GetCache<TKey, TValue>(object rootKey)
        where TKey : notnull =>
        this.cache.GetOrAddAs(rootKey, _ => new Cache<TKey, TValue>(rootKey));
}