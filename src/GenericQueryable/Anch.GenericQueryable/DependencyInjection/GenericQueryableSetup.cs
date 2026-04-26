using Anch.DependencyInjection;
using Anch.GenericQueryable.Fetching;
using Anch.GenericQueryable.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anch.GenericQueryable.DependencyInjection;

public class GenericQueryableSetup : IGenericQueryableSetup, IServiceInitializer
{
    private Type fetchServiceType = typeof(IgnoreFetchService);

    private Type targetMethodExtractorType = typeof(QueryableTargetMethodExtractor);

    private readonly List<Type> fetchRuleExpanderTypeList = [];

    private readonly List<FetchRuleHeaderInfo> fetchRuleHeaderInfoList = [];

    public void Initialize(IServiceCollection services)
    {
        if (!services.AlreadyInitialized<IGenericQueryableExecutor>())
        {
            services.TryAddSingleton<IGenericQueryableExecutor, GenericQueryableExecutor>();
            services.TryAddSingleton<IMethodRedirector, MethodRedirector>();

            services.AddKeyedSingleton<IFetchRuleExpander, FetchRuleHeaderExpander>(IFetchRuleExpander.ElementKey);
            services.AddKeyedSingleton<IFetchRuleExpander, UntypedFetchExpander>(IFetchRuleExpander.ElementKey);

            services.AddSingleton<IFetchRuleExpander, RootFetchRuleExpander>();
        }

        if (services.AlreadyInitialized<IFetchService, IgnoreFetchService>())
        {
            services.ReplaceSingleton(typeof(IFetchService), this.fetchServiceType);
        }
        else
        {
            services.AddSingleton(typeof(IFetchService), this.fetchServiceType);
        }

        if (services.AlreadyInitialized<ITargetMethodExtractor, QueryableTargetMethodExtractor>())
        {
            services.ReplaceSingleton(typeof(ITargetMethodExtractor), this.targetMethodExtractorType);
        }
        else
        {
            services.AddSingleton(typeof(ITargetMethodExtractor), this.targetMethodExtractorType);
        }

        foreach (var fetchRuleExpanderType in this.fetchRuleExpanderTypeList)
        {
            services.AddKeyedSingleton(typeof(IFetchRuleExpander), IFetchRuleExpander.ElementKey, fetchRuleExpanderType);
        }

        foreach (var fetchRuleHeaderInfo in this.fetchRuleHeaderInfoList)
        {
            services.AddSingleton(fetchRuleHeaderInfo);
        }
    }

    public IGenericQueryableSetup SetFetchService<TFetchService>()
        where TFetchService : IFetchService
    {
        this.fetchServiceType = typeof(TFetchService);

        return this;
    }

    public IGenericQueryableSetup AddFetchRule<TSource>(FetchRuleHeader<TSource> header, PropertyFetchRule<TSource> implementation)
    {
        this.fetchRuleHeaderInfoList.Add(new FetchRuleHeaderInfo<TSource>(header, implementation));

        return this;
    }

    public IGenericQueryableSetup SetTargetMethodExtractor<TTargetMethodExtractor>()
        where TTargetMethodExtractor : ITargetMethodExtractor
    {
        this.targetMethodExtractorType = typeof(TTargetMethodExtractor);

        return this;
    }

    public IGenericQueryableSetup AddFetchRuleExpander<TFetchRuleExpander>()
        where TFetchRuleExpander : IFetchRuleExpander
    {
        this.fetchRuleExpanderTypeList.Add(typeof(TFetchRuleExpander));

        return this;
    }
}