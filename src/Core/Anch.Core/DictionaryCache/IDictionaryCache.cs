namespace Anch.Core.DictionaryCache;

/// <summary>A read-only view of a dictionary whose values are created on first access.</summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public interface IDictionaryCache<in TKey, out TValue>
{
    /// <summary>The values created so far.</summary>
    IEnumerable<TValue> Values { get; }

    /// <summary>Returns the value of a key, creating it on first access.</summary>
    /// <param name="key">The key.</param>
    TValue this[TKey key] { get; }
}