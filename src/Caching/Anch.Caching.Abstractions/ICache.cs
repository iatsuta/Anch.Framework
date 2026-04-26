namespace Anch.Caching;

public interface ICache<TKey, TValue> : ICache
{
    TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);

    Type ICache.KeyType => typeof(TKey);

    Type ICache.ValueType => typeof(TValue);
}

public interface ICache
{
    object RootKey { get; }

    Type KeyType { get; }

    Type ValueType { get; }
}