using CommonFramework;
using CommonFramework.DependencyInjection;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource.DependencyInjection;

using GenericQueryable.IntegrationTests.Environment;
using GenericQueryable.NHibernate;

using Microsoft.Extensions.DependencyInjection;

using NHibernate;

#if DEBUG
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#endif

[assembly:CommonFramework.Testing.CommonTestFramework<TestEnvironment>]

namespace GenericQueryable.IntegrationTests.Environment;

public class TestEnvironment : TestEnvironmentBase
{
    public override IServiceProvider BuildServiceProvider(IServiceCollection services) =>

        services
            .AddIdentitySource()
            .AddSingleton(BuildConfigurationHelper.BuildConfiguration("Data Source=TestSystem.sqlite"))
            .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory())
            .AddScopedFrom((ISessionFactory sessionFactory) => sessionFactory.OpenSession())

            .AddSingleton(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategy<>))
            .BindServiceProxy(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategyServiceProxyBinder<>))

            .AddScoped<IGenericRepository, NHibGenericRepository>()
            .AddScoped<IQueryableSource, NHibQueryableSource>()

            .AddScoped<IDbSchemaInitializer, DbSchemaInitializer>()

            .AddNHibernateGenericQueryable(SetupGenericQueryable)

            .Pipe(base.BuildServiceProvider);
}