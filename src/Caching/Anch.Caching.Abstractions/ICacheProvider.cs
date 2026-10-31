namespace Anch.Caching;

/// <summary>Creates named caches. Calling it again with the same name returns the same cache.</summary>
public interface ICacheProvider
{
    /// <summary>Returns the cache registered under a name.</summary>
    /// <param name="key">The name of the cache.</param>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    ICache<TKey, TValue> GetCache<TKey, TValue>(object key)
        where TKey : notnull;
}