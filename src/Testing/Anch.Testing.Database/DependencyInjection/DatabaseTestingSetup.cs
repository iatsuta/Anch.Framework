using Anch.Core;
using Anch.DependencyInjection;
using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.Initializers;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.DependencyInjection;

/// <summary>Collects the settings of database testing and turns them into registrations.</summary>
public class DatabaseTestingSetup : IDatabaseTestingSetup, IServiceInitializer
{
    private bool allowParallelization = true;

    private IDatabaseTestingProvider? databaseTestingProvider;

    private Action<IServiceCollection>? initEmptySchemaAction;

    private Action<IServiceCollection>? initTestDataAction;

    private Action<IServiceCollection>? initSettingsAction;

    private Type databaseSnapshotInitializerType = typeof(DatabaseSnapshotInitializer);

    public IDatabaseTestingSetup SetParallelization(bool allow)
    {
        this.allowParallelization = allow;

        return this;
    }

    public IDatabaseTestingSetup SetDatabaseSnapshotInitializer<TDatabaseSnapshotInitializer>()
        where TDatabaseSnapshotInitializer : IInitializer
    {
        this.databaseSnapshotInitializerType = typeof(TDatabaseSnapshotInitializer);

        return this;
    }

    public void Initialize(IServiceCollection services)
    {
        services

            .AddSingleton(new AllowParallelizationConstraint(this.allowParallelization))
            .AddSingleton<IMainServiceProviderSettings, DatabaseMainServiceProviderSettings>()
            .AddSingleton<ITestConnectionStringProvider, TestConnectionStringProvider>()
            .AddKeyedSingleton(typeof(IInitializer), ITestEnvironment.MainServiceProviderKey, this.databaseSnapshotInitializerType)
            .AddSingleton<IDatabaseSnapshotManager, DatabaseSnapshotManager>()
            .AddSingleton<ITestConnectionStringPostfixFactory, TestConnectionStringPostfixFactory>();

        (this.initEmptySchemaAction ?? throw new InvalidOperationException("Empty schema initializer is not set.")).Invoke(services);

        (this.initTestDataAction ?? throw new InvalidOperationException("Test data initializer is not set.")).Invoke(services);

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