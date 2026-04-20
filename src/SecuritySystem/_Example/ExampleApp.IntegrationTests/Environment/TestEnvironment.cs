using CommonFramework;
using CommonFramework.DependencyInjection;

using ExampleApp.Api.Controllers;
using ExampleApp.Infrastructure.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.Testing.DependencyInjection;

namespace ExampleApp.IntegrationTests.Environment;

public abstract class TestEnvironment
{
    public IServiceProvider RootServiceProvider => field ??= this.BuildServiceProvider();

    protected IServiceProvider BuildServiceProvider()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("testAppSettings.json", false, true).Build();

        return new ServiceCollection()
            .AddInfrastructure(configuration)
            .Pipe(services => this.InitializeServices(services, configuration))

            .AddScoped<TestController>()
            .AddSingleton(TimeProvider.System)
            //.ReplaceSingleton<IDefaultCancellationTokenSource, XUnitDefaultCancellationTokenSource>()

            .AddSecuritySystemTesting()

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    protected abstract IServiceCollection InitializeServices(IServiceCollection services, IConfiguration configuration);
}