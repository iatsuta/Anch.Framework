using Anch.DependencyInjection;
using Anch.GenericQueryable.IntegrationTests.Environment;
using Anch.GenericQueryable.NHibernate;
using Anch.GenericRepository;
using Anch.IdentitySource.DependencyInjection;
using Anch.Testing.Xunit;

using Microsoft.Extensions.DependencyInjection;

using NHibernate;
[assembly:AnchTestFramework<NHibTestEnvironment>]

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class NHibTestEnvironment : TestEnvironment
{
    protected override IServiceCollection AddServices(IServiceCollection services) =>

        services.AddIdentitySource()
            .AddSingleton<NHibConfigurationSource>()
            .AddSingletonFrom((NHibConfigurationSource configurationSource) => configurationSource.BuildConfiguration())
            .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory())
            .AddScopedFrom((ISessionFactory sessionFactory) => sessionFactory.OpenSession()).AddSingleton(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategy<>))
            .BindServiceProxy(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategyServiceProxyBinder<>))

            .AddScoped<IGenericRepository, NHibGenericRepository>()
            .AddScoped<IQueryableSource, NHibQueryableSource>()

            .AddSingleton<IEmptySchemaInitializer, NHibEmptySchemaInitializer>()

            .AddNHibernateGenericQueryable(new GenericQueryableSetupConfigurator().Configure);
}