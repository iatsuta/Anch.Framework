using CommonFramework;
using CommonFramework.DependencyInjection;
using CommonFramework.Testing;

using ExampleApp.Api.Controllers;
using ExampleApp.Infrastructure.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.Testing.DependencyInjection;

namespace ExampleApp.IntegrationTests.Environment;

public abstract class TestServiceProviderBuilderBase : ITestServiceProviderBuilder
{
    public IServiceProvider Build(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("testAppSettings.json", false, true).Build();

        return new ServiceCollection()
            .AddInfrastructure(configuration)
            .Pipe(s => this.InitializeServices(s, configuration))

            .AddScoped<TestController>()
            .AddSingleton(TimeProvider.System)
            .ReplaceSingleton<IDefaultCancellationTokenSource, XUnitDefaultCancellationTokenSource>()

            .AddSecuritySystemTesting()

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    protected abstract IServiceCollection InitializeServices(IServiceCollection services, IConfiguration configuration);
}