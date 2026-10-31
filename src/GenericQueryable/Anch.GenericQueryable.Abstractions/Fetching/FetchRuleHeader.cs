namespace Anch.GenericQueryable.Fetching;

/// <summary>
/// A named fetch rule: a key that stands for a set of property paths.
/// The paths are supplied by an <see cref="IFetchRuleExpander"/> registered for the key, so callers reference only the name.
/// </summary>
/// <typeparam name="TSource">The root entity type.</typeparam>
public abstract record FetchRuleHeader<TSource> : FetchRule<TSource>
{
    /// <summary>Creates a header carrying <paramref name="value"/> as its key.</summary>
    /// <param name="value">The key (usually an enum value).</param>
    /// <typeparam name="TValue">Type of the key.</typeparam>
    public static FetchRuleHeader<TSource, TValue> Create<TValue>(TValue value) => new(value);
}

/// <summary>A <see cref="FetchRuleHeader{TSource}"/> identified by a value.</summary>
/// <param name="Value">The key of the rule.</param>
/// <typeparam name="TSource">The root entity type.</typeparam>
/// <typeparam name="TValue">Type of the key.</typeparam>
public record FetchRuleHeader<TSource, TValue>(TValue Value) : FetchRuleHeader<TSource>;