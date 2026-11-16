using Anch.Core;

namespace Anch.GenericQueryable.Fetching;

/// <summary>A property fetch rule that remembers the type of the last fetched property, which enables <c>ThenFetch</c>.</summary>
/// <typeparam name="TSource">The root entity type.</typeparam>
/// <typeparam name="TLastProperty">Type of the last fetched property.</typeparam>
public interface IPropertyFetchRule<TSource, out TLastProperty> : IPropertyFetchRule<TSource>;

/// <summary>A fetch rule expressed as property paths.</summary>
/// <typeparam name="TSource">The root entity type.</typeparam>
public interface IPropertyFetchRule<TSource>
{
    /// <summary>The paths to load; each path is a chain of nested properties.</summary>
    DeepEqualsCollection<LambdaExpressionPath> Paths { get; }
}