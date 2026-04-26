using Anch.DependencyInjection;
using ExampleApp.Infrastructure.DependencyInjection.UndirectView;
using ExampleApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddNHibernateInfrastructure(IConfiguration configuration)
        {
            return Anch.GenericQueryable.NHibernate.ServiceCollectionExtensions.AddNHibernateGenericQueryable(services
                    .AddSingleton(new ViewSchema("app"))

                    .AddSingleton<NHibConfigurationSource>()
                    .AddSingletonFrom((NHibConfigurationSource configurationSource) => configurationSource.BuildConfiguration())
                    .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory())

                    .AddSingleton(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategy<>))
                    .BindServiceProxy(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategyServiceProxyBinder<>))

                    .AddScoped(typeof(IDal<>), typeof(NHibDal<>))
                    .AddScoped<NHibAutoCommitSession>()
                    .AddSingleton<IEmptySchemaInitializer, NHibEmptySchemaInitializer>()

                    .AddSingleton<INHibExpressionVisitorSource, NHibExpressionVisitorSource>());
        }
    }
}