namespace Anch.Caching;

public static class CacheExtensions
{
    public static TNestedValue GetOrAddAs<TKey, TValue, TNestedValue>(
        this ICache<TKey, TValue> cache, TKey key,
        Func<TKey, TNestedValue> valueFactory)
        where TKey : notnull
        where TValue : class
        where TNestedValue : TValue
        => (TNestedValue)cache.GetOrAdd(key, v => valueFactory(v));
}