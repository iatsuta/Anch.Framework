namespace Anch.Core.DictionaryCache;

/// <summary>A dictionary that creates values on first access and detects cycles in their creation. Not thread-safe; wrap it with <c>WithLock</c>.</summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public class DictionaryCache<TKey, TValue> : IDictionaryCache<TKey, TValue>
    where TKey : notnull
{
    private readonly Func<TKey, IDictionaryCache<TKey, TValue>, TValue> getNewValue;

    private readonly Dictionary<TKey, TValue> dict;

    private readonly HashSet<TKey> calcKeys;


    /// <summary>Creates the cache.</summary>
    /// <param name="getNewValue">Creates the value of a key.</param>
    /// <param name="comparer">The key comparer; <c>null</c> for the default one.</param>
    public DictionaryCache(Func<TKey, TValue> getNewValue, IEqualityComparer<TKey>? comparer = null)
        : this((key, _) => getNewValue(key), comparer)
    {
    }

    /// <summary>Creates the cache; the factory may read other values from the cache, but a cycle throws.</summary>
    /// <param name="getNewValue">Creates the value of a key.</param>
    /// <param name="comparer">The key comparer; <c>null</c> for the default one.</param>
    public DictionaryCache(Func<TKey, IDictionaryCache<TKey, TValue>, TValue> getNewValue, IEqualityComparer<TKey>? comparer = null)
    {
        this.getNewValue = getNewValue;
        this.Comparer = comparer ?? EqualityComparer<TKey>.Default;

        this.dict = new Dictionary<TKey, TValue>(this.Comparer);
        this.calcKeys = new HashSet<TKey>(this.Comparer);
    }

    /// <summary>The key comparer.</summary>
    public IEqualityComparer<TKey> Comparer { get; }


    /// <summary>Returns the value of a key, creating it on first access.</summary>
    /// <param name="key">The key.</param>
    /// <exception cref="InvalidOperationException">The value of the key depends on itself.</exception>
    public TValue this[TKey key]
    {
        get
        {
            return this.dict.GetValueOrCreate(key, () =>
            {
                if (this.calcKeys.Add(key))
                {
                    try
                    {
                        return this.getNewValue(key, this);
                    }
                    finally
                    {
                        this.calcKeys.Remove(key);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"duplicate calc key: {key}");
                }
            });
        }
    }

    IEnumerable<TValue> IDictionaryCache<TKey, TValue>.Values => this.dict.Values;
}