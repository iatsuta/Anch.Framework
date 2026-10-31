using System.Linq.Expressions;

using Anch.Core;

namespace Anch.GenericQueryable.Fetching;

/// <summary>A fetch rule made of typed property paths.</summary>
/// <param name="Paths">The paths to load.</param>
/// <typeparam name="TSource">The root entity type.</typeparam>
public record PropertyFetchRule<TSource>(DeepEqualsCollection<LambdaExpressionPath> Paths) : FetchRule<TSource>, IPropertyFetchRule<TSource>
{
    public PropertyFetchRule(IEnumerable<LambdaExpressionPath> paths) :
        this(DeepEqualsCollection.Create(paths))
    {
    }

    /// <summary>Adds one more root-level path to load.</summary>
    /// <param name="path">The property to load.</param>
    /// <typeparam name="TNextProperty">Type of the property.</typeparam>
    public PropertyFetchRule<TSource, TNextProperty> Fetch<TNextProperty>(Expression<Func<TSource, TNextProperty>> path)
    {
        return new PropertyFetchRule<TSource, TNextProperty>(this.Paths.Concat([new LambdaExpressionPath([path])]));
    }
}

/// <summary>A <see cref="PropertyFetchRule{TSource}"/> that remembers the type of its last property so that <c>ThenFetch</c> can continue the path.</summary>
/// <param name="Paths">The paths to load.</param>
/// <typeparam name="TSource">The root entity type.</typeparam>
/// <typeparam name="TLastProperty">Type of the last fetched property.</typeparam>
public record PropertyFetchRule<TSource, TLastProperty>(DeepEqualsCollection<LambdaExpressionPath> Paths)
    : PropertyFetchRule<TSource>(Paths), IPropertyFetchRule<TSource, TLastProperty>
{
    public PropertyFetchRule(IEnumerable<LambdaExpressionPath> paths) :
        this(DeepEqualsCollection.Create(paths))
    {
    }
}