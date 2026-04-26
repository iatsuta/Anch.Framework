using Anch.Testing;
using ExampleApp.Infrastructure.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: AnchTestFramework<ExampleApp.IntegrationTests.Environment.NHibTestEnvironment>]

namespace ExampleApp.IntegrationTests.Environment;

public class NHibTestEnvironment : TestEnvironment
{
    protected override IServiceCollection InitializeServices(IServiceCollection services, IConfiguration configuration) =>
        services.AddNHibernateInfrastructure(configuration)
            .AddSingleton(configuration);
}