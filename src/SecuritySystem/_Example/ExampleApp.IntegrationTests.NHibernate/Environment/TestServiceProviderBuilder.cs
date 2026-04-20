using ExampleApp.Infrastructure.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: CommonFramework.Testing.CommonTestFramework<ExampleApp.IntegrationTests.Environment.TestServiceProviderBuilder>]

namespace ExampleApp.IntegrationTests.Environment;

public class TestServiceProviderBuilder : TestServiceProviderBuilderBase
{
    protected override IServiceCollection InitializeServices(IServiceCollection services, IConfiguration configuration) =>
        services.AddNHibernateInfrastructure(configuration)
            .AddSingleton(configuration);
}