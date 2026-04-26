using System.Collections.Immutable;

namespace Anch.Core;

public static class DictionaryExtensions
{
    public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(
        this IEnumerable<(TKey Key, TValue Value)> source)
        where TKey : notnull => source.ToImmutableDictionary(pair => pair.Key, pair => pair.Value);

    extension<TKey, TValue>(IDictionary<TKey, TValue> source)
    {
        public TValue GetValueOrCreate(TKey key, Func<TValue> getNewValue)
        {
            if (source.TryGetValue(key, out var value))
            {
                return value;
            }

            value = getNewValue();

            source.Add(key, value);

            return value;
        }

        public async ValueTask<TValue> GetValueOrCreateAsync(TKey key, Func<TKey, ValueTask<TValue>> getNewValue)
        {
            if (!source.TryGetValue(key, out var value))
            {
                value = await getNewValue(key);

                source[key] = value;
            }

            return value;
        }

        public ValueTask<TValue> GetValueOrCreateAsync(TKey key, Func<ValueTask<TValue>> getNewValue) =>
            source.GetValueOrCreateAsync(key, _ => getNewValue());
    }

    extension<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> source)
        where TKey : notnull
    {
        public TValue GetValueOrDefault(TKey key, Func<TValue> getDefaultValueFunc)
        {
            return source.TryGetValue(key, out var value) ? value : getDefaultValueFunc();
        }

        public TValue GetValue(TKey key, Func<Exception> getKeyNotFoundError)
        {
            return source.GetValueOrDefault(key, () => throw getKeyNotFoundError());
        }

        public Maybe<TValue> GetMaybeValue(TKey key)
        {
            return source.TryGetValue(key, out var value) ? Maybe.Return(value) : Maybe<TValue>.Nothing;
        }

        public Dictionary<TKey, TValue> Concat(IReadOnlyDictionary<TKey, TValue> other)
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)source).Concat(other).ToDictionary();
        }

        public Dictionary<TNewKey, TValue> ChangeKey<TNewKey>(Func<TKey, TNewKey> selector)
            where TNewKey : notnull
        {
            return source.ToDictionary(pair => selector(pair.Key), pair => pair.Value);
        }

        public Dictionary<TKey, TNewValue> ChangeValue<TNewValue>(Func<TValue, TNewValue> selector)
        {
            return source.ToDictionary(pair => pair.Key, pair => selector(pair.Value));
        }
    }
}