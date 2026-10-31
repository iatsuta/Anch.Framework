namespace Anch.GenericQueryable.Fetching;

/// <summary>Extensions for <see cref="IFetchRuleExpander"/>.</summary>
public static class FetchRuleExpanderExtensions
{
    /// <summary>Expands a rule into concrete property paths; throws when no expander supports the rule.</summary>
    /// <param name="fetchRuleExpander">The expander.</param>
    /// <param name="fetchRule">The rule to expand.</param>
    /// <exception cref="ArgumentOutOfRangeException">The rule cannot be expanded.</exception>
    public static PropertyFetchRule<TSource> Expand<TSource>(this IFetchRuleExpander fetchRuleExpander, FetchRule<TSource> fetchRule) =>
        fetchRuleExpander.TryExpand(fetchRule) ?? throw new ArgumentOutOfRangeException(nameof(fetchRule));
}