using CommonFramework;
using CommonFramework.DependencyInjection;
using CommonFramework.Testing.Database;
using CommonFramework.Testing.Database.DependencyInjection;
using CommonFramework.Testing.Database.Sqlite;

using Microsoft.Extensions.DependencyInjection;

namespace GenericQueryable.IntegrationTests.Environment;

public abstract class TestEnvironment : ITestEnvironment
{
    protected abstract IServiceCollection AddServices(IServiceCollection services);

    public IServiceProvider BuildServiceProvider(IServiceCollection services) =>

        services

            .AddSingleton<IGenericQueryableSetupConfigurator, GenericQueryableSetupConfigurator>()

            .Pipe(this.AddServices)

            .AddSingleton<ISharedTestDataInitializer, SharedTestDataInitializer>()

            .AddDatabaseTesting(dts => dts
                .SetProvider<SqliteDatabaseTestingProvider>()
                .SetEmptySchemaInitializer<IEmptySchemaInitializer>()
                .SetSharedTestDataInitializer<ISharedTestDataInitializer>()
                .SetSettings(new TestDatabaseSettings
                {
                    InitMode = DatabaseInitModeHelper.DatabaseInitMode,
                    DefaultConnectionString = new("Data Source=test.db;Pooling=False")
                })
                .RebindActualConnection<IMainConnectionStringSource>(connectionString => new MainConnectionStringSource(connectionString.Value)))

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
}