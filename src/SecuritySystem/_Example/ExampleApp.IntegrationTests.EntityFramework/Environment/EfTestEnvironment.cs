using Anch.Testing;
using ExampleApp.Infrastructure.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: AnchTestFramework<ExampleApp.IntegrationTests.Environment.EfTestEnvironment>]

namespace ExampleApp.IntegrationTests.Environment;

public class EfTestEnvironment : TestEnvironment
{
    protected override IServiceCollection InitializeServices(IServiceCollection services, IConfiguration configuration) =>
        services.AddEntityFrameworkInfrastructure(configuration);
}