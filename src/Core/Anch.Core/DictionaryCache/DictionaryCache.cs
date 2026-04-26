namespace Anch.Core.DictionaryCache;

public class DictionaryCache<TKey, TValue> : IDictionaryCache<TKey, TValue>
    where TKey : notnull
{
    private readonly Func<TKey, IDictionaryCache<TKey, TValue>, TValue> getNewValue;

    private readonly Dictionary<TKey, TValue> dict;

    private readonly HashSet<TKey> calcKeys;


    public DictionaryCache(Func<TKey, TValue> getNewValue, IEqualityComparer<TKey>? comparer = null)
        : this((key, _) => getNewValue(key), comparer)
    {
    }

    public DictionaryCache(Func<TKey, IDictionaryCache<TKey, TValue>, TValue> getNewValue, IEqualityComparer<TKey>? comparer = null)
    {
        this.getNewValue = getNewValue;
        this.Comparer = comparer ?? EqualityComparer<TKey>.Default;

        this.dict = new Dictionary<TKey, TValue>(this.Comparer);
        this.calcKeys = new HashSet<TKey>(this.Comparer);
    }

    public IEqualityComparer<TKey> Comparer { get; }


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