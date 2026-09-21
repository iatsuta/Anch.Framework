using Anch.GenericQueryable.Fetching;
using Anch.GenericQueryable.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.DependencyInjection;

public interface IGenericQueryableSetup : IGenericQueryableSetup<IGenericQueryableSetup>
{
    IGenericQueryableSetup SetFetchService<TFetchService>()
        where TFetchService : IFetchService;

    IGenericQueryableSetup AddFetchRuleExpander<TFetchRuleExpander>()
        where TFetchRuleExpander : IFetchRuleExpander;

    IGenericQueryableSetup AddFetchRule<TSource>(FetchRuleHeader<TSource> header, PropertyFetchRule<TSource> implementation);

    IGenericQueryableSetup SetTargetMethodExtractor<TTargetMethodExtractor>()
        where TTargetMethodExtractor : ITargetMethodExtractor;
}

public interface IGenericQueryableSetup<out TSelf>
    where TSelf : IGenericQueryableSetup<TSelf>
{
    TSelf AddExtension(IGenericQueryableExtension extension);

    TSelf AddExtension<TGenericQueryableExtension>()
        where TGenericQueryableExtension : IGenericQueryableExtension, new() =>
        this.AddExtension(new TGenericQueryableExtension());

    TSelf AddServices(Action<IServiceCollection> setupAction) => this.AddExtension(new GenericQueryableExtension(setupAction));
}