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

    private Action<IServiceCollection>? initSharedTestDataAction;

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
            .AddKeyedSingleton<IInitializer, CachedSharedTestDataInitializer>(TestDatabaseInitializer.CachedSharedTestDataKey)

            .AddSingleton<IDatabaseManager, FileDatabaseManager>();

        (this.initEmptySchemaAction ?? throw new InvalidOperationException("Empty schema initializer is not set.")).Invoke(services);

        (this.initSharedTestDataAction ?? throw new InvalidOperationException("Shared test data initializer is not set.")).Invoke(services);

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
        this.initSharedTestDataAction = sc =>
        {
            if (register)
            {
                sc.AddKeyedSingleton<IInitializer, TEmptySchemaInitializer>(TestDatabaseInitializer.EmptySchemaKey);
            }
            else
            {
                sc.AddKeyedSingleton<IInitializer>(TestDatabaseInitializer.EmptySchemaKey, (sp, _) => sp.GetRequiredService<TEmptySchemaInitializer>());
            }
        };

        return this;
    }

    public IDatabaseTestingSetup SetSharedTestDataInitializer<TSharedTestDataInitializer>(bool register = true)
        where TSharedTestDataInitializer : class, IInitializer
    {
        this.initSharedTestDataAction = sc =>
        {
            if (register)
            {
                sc.AddKeyedSingleton<IInitializer, TSharedTestDataInitializer>(TestDatabaseInitializer.SharedTestDataKey);
            }
            else
            {
                sc.AddKeyedSingleton<IInitializer>(TestDatabaseInitializer.SharedTestDataKey, (sp, _) => sp.GetRequiredService<TSharedTestDataInitializer>());
            }
        };

        return this;
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