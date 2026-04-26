using System.Diagnostics;
using CommonFramework;
using CommonFramework.DependencyInjection;
using CommonFramework.Testing.Database;
using CommonFramework.Testing.Database.DependencyInjection;
using CommonFramework.Testing.Database.Sqlite;

using ExampleApp.Api.Controllers;
using ExampleApp.Infrastructure.DependencyInjection;
using ExampleApp.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.Testing.DependencyInjection;

namespace ExampleApp.IntegrationTests.Environment;

public abstract class TestEnvironment : ITestEnvironment
{
    public IServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("testAppSettings.json", false, true).Build();

        return services
            .AddInfrastructure(configuration)
            .Pipe(s => this.InitializeServices(s, configuration))

            .AddScoped<TestController>()
            .AddSingleton(TimeProvider.System)
            .ReplaceSingleton<IDefaultCancellationTokenSource, XUnitDefaultCancellationTokenSource>()

            .AddSecuritySystemTesting()

            .AddDatabaseTesting(dts => dts
                .SetProvider<SqliteDatabaseTestingProvider>()
                .SetEmptySchemaInitializer<IEmptySchemaInitializer>()
                .SetSharedTestDataInitializer<ISharedTestDataInitializer>()
                .SetSettings(new TestDatabaseSettings
                {
                    InitMode = DatabaseInitModeHelper.DatabaseInitMode,
                    DefaultConnectionString = new (configuration.GetRequiredConnectionString(ConfigurationMainConnectionStringSource.DefaultName))
                })
                .RebindActualConnection<IMainConnectionStringSource>(connectionString => new MainConnectionStringSource(connectionString.Value)))

            .AddEnvironmentHook(EnvironmentHookType.After, sp => sp.GetRequiredService<RootImpersonateServiceState>().Reset())

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    protected abstract IServiceCollection InitializeServices(IServiceCollection services, IConfiguration configuration);
}