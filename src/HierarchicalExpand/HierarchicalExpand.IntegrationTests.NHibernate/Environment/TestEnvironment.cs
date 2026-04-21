using CommonFramework.DependencyInjection;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource.DependencyInjection;

using GenericQueryable.NHibernate;
using HierarchicalExpand.IntegrationTests.Environment.UndirectView;
using Microsoft.Extensions.DependencyInjection;

#if DEBUG
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#endif

[assembly: CommonFramework.Testing.CommonTestFramework<HierarchicalExpand.IntegrationTests.Environment.TestEnvironment>]

namespace HierarchicalExpand.IntegrationTests.Environment;

public class TestEnvironment : TestEnvironmentBase
{
    private readonly string dbName = "test.db";

    protected override IServiceCollection InitializeServices(IServiceCollection services)
    {
        return services

            .AddSingleton(new ViewSchema("app"))

            .AddIdentitySource()
            .AddSingleton(BuildConfigurationHelper.BuildConfiguration($"Data Source={this.dbName}"))
            .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory())

            .AddScoped<AutoCommitSession>()

            .AddSingleton(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategy<>))
            .BindServiceProxy(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategyServiceProxyBinder<>))

            .AddScoped<IGenericRepository, NHibGenericRepository>()
            .AddScoped<IQueryableSource, NHibQueryableSource>()

            .AddScoped<IDbSchemaInitializer, DbSchemaInitializer>()

            .AddNHibernateGenericQueryable();
    }
}