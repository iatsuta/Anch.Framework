namespace GenericQueryable.Fetching;

public interface IFetchRuleExpander
{
    public const string ElementKey = "Element";

    PropertyFetchRule<TSource>? TryExpand<TSource>(FetchRule<TSource> fetchRule);
}