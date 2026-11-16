using System.Linq.Expressions;

using Anch.Core;
using Anch.GenericQueryable.Fetching;

namespace Anch.GenericQueryable;

/// <summary>Fluent helpers for building <see cref="PropertyFetchRule{TSource}"/> chains.</summary>
public static class PropertyFetchRuleExtensions
{
    /// <summary>Builds a fetch rule by running the builder against <see cref="FetchRule{TSource}.Empty"/>.</summary>
    /// <param name="buildFetchRule">Fluent rule builder.</param>
    public static FetchRule<TSource> ToFetchRule<TSource>(this Func<PropertyFetchRule<TSource>, FetchRule<TSource>> buildFetchRule)
    {
        return buildFetchRule(FetchRule<TSource>.Empty);
    }

    /// <summary>Extends the last fetch path with a property of the previously fetched object.</summary>
    /// <param name="fetchRule">The rule whose last path is extended.</param>
    /// <param name="prop">Property to load next.</param>
    public static PropertyFetchRule<TSource, TNextProperty> ThenFetch<TSource, TLastProperty, TNextProperty>(this IPropertyFetchRule<TSource, TLastProperty> fetchRule,
        Expression<Func<TLastProperty, TNextProperty>> prop)
    {
        return fetchRule.ThenFetchInternal<TSource, TNextProperty>(prop);
    }

    /// <summary>Extends the last fetch path with a property of the elements of the previously fetched collection.</summary>
    /// <param name="fetchRule">The rule whose last path is extended.</param>
    /// <param name="prop">Property of a collection element to load next.</param>
    public static PropertyFetchRule<TSource, TNextProperty> ThenFetch<TSource, TLastProperty, TNextProperty>(this IPropertyFetchRule<TSource, IEnumerable<TLastProperty>> fetchRule,
        Expression<Func<TLastProperty, TNextProperty>> prop)
    {
        return fetchRule.ThenFetchInternal<TSource, TNextProperty>(prop);
    }

    private static PropertyFetchRule<TSource, TNextProperty> ThenFetchInternal<TSource, TNextProperty>(this IPropertyFetchRule<TSource> fetchRule, LambdaExpression prop)
    {
        var prevPaths = fetchRule.Paths.SkipLast(1);

        var lastPath = fetchRule.Paths.Last();

        var newLastPath = new LambdaExpressionPath(lastPath.Properties.Concat([prop]).ToList());

        return new PropertyFetchRule<TSource, TNextProperty>([.. prevPaths, newLastPath]);
    }
}