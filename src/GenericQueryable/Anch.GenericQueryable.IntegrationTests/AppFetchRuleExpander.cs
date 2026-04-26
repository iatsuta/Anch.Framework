using Anch.GenericQueryable.Fetching;

namespace Anch.GenericQueryable.IntegrationTests;

public class AppFetchRuleExpander : IFetchRuleExpander
{
    public PropertyFetchRule<TSource>? TryExpand<TSource>(FetchRule<TSource> fetchRule) => null;
}