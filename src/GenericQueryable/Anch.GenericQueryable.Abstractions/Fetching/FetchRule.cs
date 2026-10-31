using System.Linq.Expressions;

using Anch.Core;

namespace Anch.GenericQueryable.Fetching;

/// <summary>
/// Describes which related data to load together with <typeparamref name="TSource"/> (eager loading).
/// Rules are values: two rules with the same content are equal. They are applied with <c>WithFetch</c>.
/// </summary>
/// <typeparam name="TSource">The root entity type.</typeparam>
public abstract record FetchRule<TSource>
{
    /// <summary>Creates a rule from a dotted property path.</summary>
    /// <param name="path">Dotted property path, for example <c>"Orders.Items"</c>.</param>
    public static FetchRule<TSource> Create(string path)
    {
        return new UntypedFetchRule<TSource>(path);
    }

    /// <summary>Creates a rule that loads a single property.</summary>
    /// <param name="prop">The property to load.</param>
    public static PropertyFetchRule<TSource, TProperty> Create<TProperty>(Expression<Func<TSource, TProperty>> prop)
    {
        return new PropertyFetchRule<TSource, TProperty>([new LambdaExpressionPath([prop])]);
    }

    /// <summary>A rule that loads nothing; the starting point of fluent building.</summary>
    public static PropertyFetchRule<TSource> Empty { get; } = new([]);
}