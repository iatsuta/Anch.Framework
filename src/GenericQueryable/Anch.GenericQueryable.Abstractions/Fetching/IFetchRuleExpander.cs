namespace Anch.GenericQueryable.Fetching;

/// <summary>
/// Turns abstract fetch rules (headers, untyped paths, composites) into concrete <see cref="PropertyFetchRule{TSource}"/>s.
/// Several expanders can be registered; each handles the rules it recognizes.
/// </summary>
public interface IFetchRuleExpander
{
    /// <summary>Key under which the expanders that make up the composite are registered as keyed services.</summary>
    public const string ElementKey = "Element";

    /// <summary>Expands the rule if this expander supports it.</summary>
    /// <param name="fetchRule">The rule to expand.</param>
    /// <typeparam name="TSource">The root entity type.</typeparam>
    /// <returns>The expanded rule, or <c>null</c> when the rule is not supported.</returns>
    PropertyFetchRule<TSource>? TryExpand<TSource>(FetchRule<TSource> fetchRule);
}