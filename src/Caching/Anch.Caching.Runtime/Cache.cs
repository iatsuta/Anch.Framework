using System.Collections.Concurrent;

namespace Anch.Caching;

/// <summary>An in-memory thread-safe cache built on a concurrent dictionary.</summary>
/// <param name="rootKey">The name the cache is registered under.</param>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public class Cache<TKey, TValue>(object rootKey) : ConcurrentDictionary<TKey, TValue>, ICache<TKey, TValue>
    where TKey : notnull
{
    /// <inheritdoc/>
    public object RootKey { get; } = rootKey;
}