using System.Collections.Concurrent;

using Anch.Core;

namespace Anch.Caching;

/// <summary>The default in-memory cache provider: one cache per name, kept for the lifetime of the provider.</summary>
public class CacheProvider : ICacheProvider
{
    private readonly ConcurrentDictionary<object, ICache> cache = [];

    /// <inheritdoc/>
    public ICache<TKey, TValue> GetCache<TKey, TValue>(object rootKey)
        where TKey : notnull =>
        this.cache.GetOrAddAs(rootKey, _ => new Cache<TKey, TValue>(rootKey));
}