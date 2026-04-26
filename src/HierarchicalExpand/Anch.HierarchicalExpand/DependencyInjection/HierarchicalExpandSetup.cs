using Anch.DependencyInjection;
using Anch.HierarchicalExpand.Denormalization;
using Anch.IdentitySource.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.DependencyInjection;

public class HierarchicalExpandSetup : IHierarchicalExpandSetup, IServiceInitializer
{
    private readonly List<Action<IServiceCollection>> actions = [];

    public void Initialize(IServiceCollection services)
    {
        services.AddIdentitySource();

        foreach (var action in this.actions)
        {
            action(services);
        }

        if (!services.AlreadyInitialized<IHierarchicalInfoSource>())
        {
            this.RegisterGeneralServices(services);
        }
    }

    public IHierarchicalExpandSetup AddHierarchicalInfo<TDomainObject>(
        HierarchicalInfo<TDomainObject> hierarchicalInfo,
        FullAncestorLinkInfo<TDomainObject> fullAncestorLinkInfo,
        DeepLevelInfo<TDomainObject>? deepLevelInfo = null)
    {
        this.actions.Add(services =>
        {
            services.AddSingleton<HierarchicalInfo>(hierarchicalInfo);
            services.AddSingleton(hierarchicalInfo);

            services.AddSingleton<FullAncestorLinkInfo>(fullAncestorLinkInfo);
            services.AddSingleton(fullAncestorLinkInfo);

            var directLinkType =
                typeof(FullAncestorLinkInfo<,>).MakeGenericType(fullAncestorLinkInfo.DomainObjectType, fullAncestorLinkInfo.DirectedLinkType);

            var withUndirectLinkType =
                typeof(FullAncestorLinkInfo<,,>).MakeGenericType(fullAncestorLinkInfo.DomainObjectType, fullAncestorLinkInfo.DirectedLinkType,
                    fullAncestorLinkInfo.UndirectedLinkType);

            services.AddSingleton(directLinkType, fullAncestorLinkInfo);
            services.AddSingleton(withUndirectLinkType, fullAncestorLinkInfo);

            if (deepLevelInfo != null)
            {
                services.AddSingleton<DeepLevelInfo>(deepLevelInfo);
                services.AddSingleton(deepLevelInfo);
            }
        });

        return this;
    }

    private IServiceCollection RegisterGeneralServices(IServiceCollection services)
    {
        return services
            .AddServiceProxyFactory()
            .AddScoped<IDeepLevelDenormalizer, DeepLevelDenormalizer>()
            .AddScoped(typeof(IDeepLevelDenormalizer<>), typeof(DeepLevelDenormalizer<>))
            .AddScoped<IAncestorDenormalizer, AncestorDenormalizer>()

            .BindServiceProxy(typeof(IAncestorDenormalizer<>), typeof(AncestorDenormalizerServiceProxyBinder<>))
            .AddScoped(typeof(IAncestorDenormalizer<>), typeof(AncestorDenormalizer<>))

            .AddScoped(typeof(IAncestorLinkExtractor<,>), typeof(AncestorLinkExtractor<,>))
            .AddSingleton<IActualDomainTypeResolver, IdentityActualTypeResolver>()
            .AddSingleton<IHierarchicalObjectExpanderTypeResolver, HierarchicalObjectExpanderTypeResolver>()
            .AddScoped<IHierarchicalObjectExpanderFactory, HierarchicalObjectExpanderFactory>()
            .AddScoped(typeof(IDomainObjectExpanderFactory<>), typeof(DomainObjectExpanderFactory<>))
            .AddSingleton<IHierarchicalInfoSource, HierarchicalInfoSource>();
    }
}