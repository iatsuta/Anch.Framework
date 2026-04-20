using CommonFramework;
using CommonFramework.DependencyInjection;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource.DependencyInjection;

using GenericQueryable.IntegrationTests.Environment;
using GenericQueryable.NHibernate;

using Microsoft.Extensions.DependencyInjection;

using NHibernate;

[assembly:CommonFramework.Testing.CommonTestFramework<TestServiceProviderBuilder>]

namespace GenericQueryable.IntegrationTests.Environment;

public class TestServiceProviderBuilder : TestServiceProviderBuilderBase
{
    public override IServiceProvider Build(IServiceCollection services) =>

        services
            .AddIdentitySource()
            .AddSingleton(BuildConfigurationHelper.BuildConfiguration("Data Source=TestSystem.sqlite"))
            .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory())
            .AddScopedFrom((ISessionFactory sessionFactory) => sessionFactory.OpenSession())

            .AddSingleton(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategy<>))
            .BindServiceProxy(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategyServiceProxyBinder<>))

            .AddScoped<IGenericRepository, NHibGenericRepository>()
            .AddScoped<IQueryableSource, NHibQueryableSource>()

            .AddScoped<IDbSchemeInitializer, DbSchemeInitializer>()

            .AddNHibernateGenericQueryable(SetupGenericQueryable)

            .Pipe(base.Build);
}