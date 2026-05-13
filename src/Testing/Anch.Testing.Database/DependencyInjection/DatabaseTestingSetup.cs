using Anch.Core;
using Anch.DependencyInjection;
using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.Hooks;
using Anch.Testing.Database.Initializers;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.DependencyInjection;

public class DatabaseTestingSetup : IDatabaseTestingSetup, IServiceInitializer
{
    private bool allowParallelization = true;

    public IDatabaseTestingSetup SetParallelization(bool allow)
    {
        this.allowParallelization = allow;

        return this;
    }


    private IDatabaseTestingProvider? databaseTestingProvider;

    private Action<IServiceCollection>? initEmptySchemaAction;

    private Action<IServiceCollection>? initTestDataAction;

    private Action<IServiceCollection>? initSettingsAction;

    public void Initialize(IServiceCollection services)
    {
        services

            .AddSingletonFrom((TestDatabaseSettings settings) =>
                new MainServiceProviderSettings(this.allowParallelization, settings.InitMode == DatabaseInitMode.External))

            .AddSingleton<ITestConnectionStringProvider, TestConnectionStringProvider>()

            .AddEnvironmentHook<PrepareDatabaseEnvironmentHook>(EnvironmentHookType.Before)
            .AddEnvironmentHook<CleanDatabaseEnvironmentHook>(EnvironmentHookType.After)

            .AddKeyedSingleton<IInitializer, DatabaseSnapshotInitializer>(IServiceProviderPool.MainServiceProviderKey)
            .AddSingleton<IDatabaseSnapshotManager, DatabaseSnapshotManager>()
            .AddSingleton<IActualConnectionStringResolver, ActualConnectionStringResolver>();

        (this.initEmptySchemaAction ?? throw new InvalidOperationException("Empty schema initializer is not set.")).Invoke(services);

        (this.initTestDataAction ?? throw new InvalidOperationException("Shared test data initializer is not set.")).Invoke(services);

        (this.initSettingsAction ?? throw new InvalidOperationException("Settings initializer is not set.")).Invoke(services);

        (this.databaseTestingProvider ?? throw new InvalidOperationException("Database testing provider is not set.")).AddServices(services);
    }

    public IDatabaseTestingSetup SetProvider(IDatabaseTestingProvider newDatabaseTestingProvider)
    {
        this.databaseTestingProvider = newDatabaseTestingProvider;

        return this;
    }

    public IDatabaseTestingSetup SetEmptySchemaInitializer<TEmptySchemaInitializer>(bool register = true)
        where TEmptySchemaInitializer : class, IInitializer
    {
        this.initEmptySchemaAction = GetIntiAction<TEmptySchemaInitializer>(TestDatabaseInitializer.EmptySchemaKey, register);

        return this;
    }

    public IDatabaseTestingSetup SetTestDataInitializer<TTestDataInitializer>(bool register = true)
        where TTestDataInitializer : class, IInitializer
    {
        this.initTestDataAction = GetIntiAction<TTestDataInitializer>(TestDatabaseInitializer.TestDataKey, register);

        return this;
    }

    private static Action<IServiceCollection> GetIntiAction<TInitializer>(string key, bool register)
        where TInitializer : class, IInitializer
    {
        return sc =>
        {
            if (register)
            {
                sc.AddKeyedSingleton<IInitializer, TInitializer>(key);
            }
            else
            {
                sc.AddKeyedSingleton<IInitializer>(key, (sp, _) => sp.GetRequiredService<TInitializer>());
            }
        };
    }

    public IDatabaseTestingSetup SetSettings(TestDatabaseSettings testDatabaseSettings)
    {
        this.initSettingsAction = sc => sc.AddSingleton(testDatabaseSettings);

        return this;
    }

    public IDatabaseTestingSetup SetSettings(Func<IServiceProvider, TestDatabaseSettings> testDatabaseSettingsFactory)
    {
        this.initSettingsAction = sc => sc.AddSingleton(testDatabaseSettingsFactory);

        return this;
    }
}