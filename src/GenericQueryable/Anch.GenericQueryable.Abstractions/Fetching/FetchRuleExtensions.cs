namespace Anch.GenericQueryable.Fetching;

public static class FetchRuleExtensions
{
    public static FetchRule<TSource> CompositeWith<TSource>(this FetchRule<TSource> left, FetchRule<TSource> right)
    {
        return new CompositeFetchRule<TSource>(left, right);
    }
}