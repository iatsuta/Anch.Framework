using Anch.DependencyInjection;
using Anch.GenericQueryable.NHibernate;
using Anch.GenericRepository;
using Anch.HierarchicalExpand.IntegrationTests.Environment;
using Anch.HierarchicalExpand.IntegrationTests.Environment.UndirectView;
using Anch.IdentitySource.DependencyInjection;
using Anch.Testing;
using Microsoft.Extensions.DependencyInjection;

[assembly: AnchTestFramework<NHibTestEnvironment>]

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class NHibTestEnvironment : TestEnvironment
{
    protected override IServiceCollection InitializeServices(IServiceCollection services)
    {
        return ServiceCollectionServiceExtensions

            .AddScoped<IGenericRepository, NHibGenericRepository>(ServiceCollectionServiceExtensions

                .AddScoped<NHibAutoCommitSession>(services

                    .AddSingleton(new ViewSchema("app"))

                    .AddIdentitySource()
                    .AddSingleton<NHibConfigurationSource>()
                    .AddSingletonFrom((NHibConfigurationSource configurationSource) => configurationSource.BuildConfiguration())
                    .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory()))

                .AddSingleton(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategy<>))
                .BindServiceProxy(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategyServiceProxyBinder<>)))
            .AddScoped<IQueryableSource, NHibQueryableSource>()

            .AddSingleton<IEmptySchemaInitializer, NHibEmptySchemaInitializer>()

            .AddNHibernateGenericQueryable();
    }
}