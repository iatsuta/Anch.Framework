namespace Anch.GenericQueryable.Fetching;

/// <summary>Extensions for composing fetch rules.</summary>
public static class FetchRuleExtensions
{
    /// <summary>Combines two rules into a <see cref="CompositeFetchRule{TSource}"/>.</summary>
    /// <param name="left">The first rule.</param>
    /// <param name="right">The second rule.</param>
    public static FetchRule<TSource> CompositeWith<TSource>(this FetchRule<TSource> left, FetchRule<TSource> right)
    {
        return new CompositeFetchRule<TSource>(left, right);
    }
}