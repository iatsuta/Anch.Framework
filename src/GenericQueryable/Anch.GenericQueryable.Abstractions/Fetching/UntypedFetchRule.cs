namespace Anch.GenericQueryable.Fetching;

/// <summary>A fetch rule given as a dotted property path string; resolved to typed paths by an <see cref="IFetchRuleExpander"/>.</summary>
/// <param name="Path">Dotted property path, for example <c>"Orders.Items"</c>.</param>
/// <typeparam name="TSource">The root entity type.</typeparam>
public sealed record UntypedFetchRule<TSource>(string Path) : FetchRule<TSource>;