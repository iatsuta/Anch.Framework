namespace Anch.Caching;

/// <summary>A typed cache.</summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public interface ICache<TKey, TValue> : ICache
{
    /// <summary>Returns the cached value for the key, creating and storing it with <paramref name="valueFactory"/> when it is missing.</summary>
    /// <param name="key">The key.</param>
    /// <param name="valueFactory">Creates the value for a missing key.</param>
    TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);

    Type ICache.KeyType => typeof(TKey);

    Type ICache.ValueType => typeof(TValue);
}

/// <summary>Untyped view of a cache.</summary>
public interface ICache
{
    /// <summary>The key under which the cache was obtained from the <see cref="ICacheProvider"/>.</summary>
    object RootKey { get; }

    /// <summary>The type of the keys.</summary>
    Type KeyType { get; }

    /// <summary>The type of the values.</summary>
    Type ValueType { get; }
}