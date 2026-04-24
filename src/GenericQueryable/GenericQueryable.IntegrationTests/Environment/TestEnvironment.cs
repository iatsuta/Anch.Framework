using CommonFramework;
using CommonFramework.DependencyInjection;
using CommonFramework.Testing;
using CommonFramework.Testing.Database;
using CommonFramework.Testing.Database.DependencyInjection;
using CommonFramework.Testing.Database.Sqlite;

using Microsoft.Extensions.DependencyInjection;

namespace GenericQueryable.IntegrationTests.Environment;

public abstract class TestEnvironment : ITestEnvironment
{
    private readonly DatabaseInitMode databaseInitMode =

#if DEBUG
        DatabaseInitMode.RebuildSnapshot;
#else
        DatabaseInitMode.RebuildSnapshot;
#endif

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
                .SetSettings(new TestDatabaseSettings { InitMode = this.databaseInitMode, DefaultConnectionString = new("Data Source=test.db;Pooling=False") })
                .RebindActualConnection<IMainConnectionStringSource>(connectionString => new MainConnectionStringSource(connectionString.Value)))

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
}