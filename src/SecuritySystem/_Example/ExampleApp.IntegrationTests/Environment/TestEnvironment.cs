using Anch.Core;
using Anch.DependencyInjection;
using Anch.SecuritySystem.Testing.DependencyInjection;
using Anch.Testing.Database;
using Anch.Testing.Database.DependencyInjection;
using Anch.Testing.Database.Sqlite;

using ExampleApp.Api.Controllers;
using ExampleApp.Infrastructure.DependencyInjection;
using ExampleApp.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.IntegrationTests.Environment;

public abstract class TestEnvironment : ITestEnvironment
{
    public IServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("testAppSettings.json", false, true).Build();

        return services
            .AddSingleton(configuration)
            .AddSingleton(TimeProvider.System)
            .AddInfrastructure(configuration)
            .Pipe(s => this.InitializeServices(s, configuration))

            .AddScoped<TestController>()
            .ReplaceSingleton<IDefaultCancellationTokenSource, XUnitDefaultCancellationTokenSource>()

            .AddSecuritySystemTesting()

            .AddDatabaseTesting(dts => dts
                .SetProvider<SqliteDatabaseTestingProvider>()
                .SetEmptySchemaInitializer<IEmptySchemaInitializer>(register: false)
                .SetTestDataInitializer<ITestDataInitializer>(register: false)
                .SetSettings(new TestDatabaseSettings
                {
                    InitMode = DatabaseInitModeHelper.DatabaseInitMode,
                    DefaultConnectionString = new (configuration.GetRequiredConnectionString(MainConnectionStringSource.DefaultName))
                })
                .RebindActualConnection<IMainConnectionStringSource>(connectionString => new ManualMainConnectionStringSource(connectionString.Value)))

            .AddEnvironmentHook(EnvironmentHookType.After, sp => sp.GetRequiredService<RootImpersonateServiceState>().Reset())

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    protected abstract IServiceCollection InitializeServices(IServiceCollection services, IConfiguration configuration);
}