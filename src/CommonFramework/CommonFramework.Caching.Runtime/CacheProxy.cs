namespace CommonFramework.Caching;

public class CacheProxy<TKey, TValue>(ICacheProvider cacheProvider) : ICache<TKey, TValue>
    where TKey : notnull
{
    private readonly ICache<TKey, TValue> realCache = cacheProvider.GetCache<TKey, TValue>(typeof(ICache<TKey, TValue>));

    public object RootKey => realCache.RootKey;

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory) => realCache.GetOrAdd(key, valueFactory);
}