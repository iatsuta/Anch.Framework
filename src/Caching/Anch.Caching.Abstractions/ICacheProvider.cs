namespace Anch.Caching;

public interface ICacheProvider
{
    ICache<TKey, TValue> GetCache<TKey, TValue>(object key)
        where TKey : notnull;
}