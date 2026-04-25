using CommonFramework.DependencyInjection;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource.DependencyInjection;

using GenericQueryable.NHibernate;

using HierarchicalExpand.IntegrationTests.Environment.UndirectView;

using Microsoft.Extensions.DependencyInjection;

#if DEBUG
[assembly: CollectionBehavior(DisableTestParallelization = false)]
#endif

[assembly: CommonFramework.Testing.CommonTestFramework<HierarchicalExpand.IntegrationTests.Environment.NHibTestEnvironment>]

namespace HierarchicalExpand.IntegrationTests.Environment;

public class NHibTestEnvironment : TestEnvironment
{
    protected override IServiceCollection InitializeServices(IServiceCollection services)
    {
        return services

            .AddSingleton(new ViewSchema("app"))

            .AddIdentitySource()
            .AddSingleton<NHibConfigurationSource>()
            .AddSingletonFrom((NHibConfigurationSource configurationSource) => configurationSource.BuildConfiguration())
            .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory())

            .AddScoped<NHibAutoCommitSession>()

            .AddSingleton(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategy<>))
            .BindServiceProxy(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategyServiceProxyBinder<>))

            .AddScoped<IGenericRepository, NHibGenericRepository>()
            .AddScoped<IQueryableSource, NHibQueryableSource>()

            .AddSingleton<IEmptySchemaInitializer, NHibEmptySchemaInitializer>()

            .AddNHibernateGenericQueryable();
    }
}