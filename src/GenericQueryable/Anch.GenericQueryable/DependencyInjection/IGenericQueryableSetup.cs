using Anch.GenericQueryable.Fetching;
using Anch.GenericQueryable.Services;

namespace Anch.GenericQueryable.DependencyInjection;

public interface IGenericQueryableSetup
{
	IGenericQueryableSetup SetFetchService<TFetchService>()
		where TFetchService : IFetchService;

    IGenericQueryableSetup AddFetchRuleExpander<TFetchRuleExpander>()
        where TFetchRuleExpander : IFetchRuleExpander;

    IGenericQueryableSetup AddFetchRule<TSource>(FetchRuleHeader<TSource> header, PropertyFetchRule<TSource> implementation);

    IGenericQueryableSetup SetTargetMethodExtractor<TTargetMethodExtractor>()
		where TTargetMethodExtractor : ITargetMethodExtractor;
}