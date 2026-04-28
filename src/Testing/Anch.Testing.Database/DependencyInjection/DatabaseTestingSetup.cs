using Anch.Core;
using Anch.DependencyInjection;
using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.Hooks;
using Anch.Testing.Database.Initializers;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.DependencyInjection;

public class DatabaseTestingSetup : IDatabaseTestingSetup, IServiceInitializer
{
    private IDatabaseTestingProvider? databaseTestingProvider;

    private Action<IServiceCollection>? initEmptySchemaAction;

    private Action<IServiceCollection>? initTestDataAction;

    private Action<IServiceCollection>? initSettingsAction;

    private Action<IServiceCollection>? initRebindConnectionStringAction;

    public void Initialize(IServiceCollection services)
    {
        services
            .AddSingleton<ITestConnectionStringProvider, TestConnectionStringProvider>()

            .AddEnvironmentHook<PrepareDatabaseEnvironmentHook>(EnvironmentHookType.Before)
            .AddEnvironmentHook<CleanDatabaseEnvironmentHook>(EnvironmentHookType.After)

            .AddSingleton(typeof(ISynchronizedInitializer<>), typeof(SynchronizedInitializer<>))

            .AddKeyedSingleton<IInitializer, CachedEmptySchemaInitializer>(TestDatabaseInitializer.CachedEmptySchemaKey)
            .AddKeyedSingleton<IInitializer, CachedTestDataInitializer>(TestDatabaseInitializer.CachedTestDataKey)

            .AddSingleton<IDatabaseManager, FileDatabaseManager>();

        (this.initEmptySchemaAction ?? throw new InvalidOperationException("Empty schema initializer is not set.")).Invoke(services);

        (this.initTestDataAction ?? throw new InvalidOperationException("Shared test data initializer is not set.")).Invoke(services);

        (this.initSettingsAction ?? throw new InvalidOperationException("Settings initializer is not set.")).Invoke(services);

        (this.initRebindConnectionStringAction ?? throw new InvalidOperationException("Rebind connection string initializer is not set.")).Invoke(services);

        (this.databaseTestingProvider ?? throw new InvalidOperationException("Database testing provider is not set.")).AddServices(services);
    }

    public IDatabaseTestingSetup SetProvider<TDatabaseTestingProvider>()
        where TDatabaseTestingProvider : IDatabaseTestingProvider, new()
    {
        this.databaseTestingProvider = new TDatabaseTestingProvider();

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

    public IDatabaseTestingSetup RebindActualConnection<T>(Func<TestDatabaseConnectionString, T> rebindFunc)
        where T : class
    {
        this.initRebindConnectionStringAction = sc => sc.ReplaceSingletonFrom((ITestConnectionStringProvider csp) => rebindFunc(csp.Actual));

        return this;
    }
}