namespace Anch.Core.DictionaryCache;

public static class DictionaryCacheExtensions
{
    public static TValue GetValue<TK1, TK2, TValue>(this IDictionaryCache<Tuple<TK1, TK2>, TValue> cache, TK1 tk1, TK2 tk2)
    {
        return cache[Tuple.Create(tk1, tk2)];
    }

    public static TValue GetValue<TK1, TK2, TK3, TValue>(this IDictionaryCache<Tuple<TK1, TK2, TK3>, TValue> cache, TK1 tk1, TK2 tk2, TK3 tk3)
    {
        return cache[Tuple.Create(tk1, tk2, tk3)];
    }

    public static IDictionaryCache<TKey, TValue> WithLock<TKey, TValue>(this IDictionaryCache<TKey, TValue> dictionaryCache, object? locker = null)
    {
        return new ConcurrentDictionaryCache<TKey, TValue>(dictionaryCache, locker ?? new object());
    }

    private class ConcurrentDictionaryCache<TKey, TValue>(IDictionaryCache<TKey, TValue> baseDictionaryCache, object locker) : IDictionaryCache<TKey, TValue>
    {
        public TValue this[TKey key]
        {
            get
            {
                lock (locker)
                {
                    return baseDictionaryCache[key];
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                lock (locker)
                {
                    return baseDictionaryCache.Values.ToArray();
                }
            }
        }
    }
}