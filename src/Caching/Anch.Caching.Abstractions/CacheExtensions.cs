namespace Anch.Caching;

/// <summary>Extensions for <see cref="ICache{TKey, TValue}"/>.</summary>
public static class CacheExtensions
{
    /// <summary>Gets or adds a value and casts it to a more specific type; use when the cache stores a base type but one key always holds a derived one.</summary>
    /// <param name="cache">The cache.</param>
    /// <param name="key">The key.</param>
    /// <param name="valueFactory">Creates the value when the key is missing.</param>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The type of the stored values.</typeparam>
    /// <typeparam name="TNestedValue">The more specific type to return.</typeparam>
    public static TNestedValue GetOrAddAs<TKey, TValue, TNestedValue>(
        this ICache<TKey, TValue> cache, TKey key,
        Func<TKey, TNestedValue> valueFactory)
        where TKey : notnull
        where TValue : class
        where TNestedValue : TValue
        => (TNestedValue)cache.GetOrAdd(key, v => valueFactory(v));
}