namespace Anch.Caching;

/// <summary>An injectable <see cref="ICache{TKey, TValue}"/>: the cache of the given key and value types, taken from the cache provider.</summary>
/// <param name="cacheProvider">Supplies the real cache.</param>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public class CacheProxy<TKey, TValue>(ICacheProvider cacheProvider) : ICache<TKey, TValue>
    where TKey : notnull
{
    private readonly ICache<TKey, TValue> realCache = cacheProvider.GetCache<TKey, TValue>(typeof(ICache<TKey, TValue>));

    /// <inheritdoc/>
    public object RootKey => this.realCache.RootKey;

    /// <inheritdoc/>
    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory) => this.realCache.GetOrAdd(key, valueFactory);
}