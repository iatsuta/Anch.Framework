using Anch.Core;

namespace Anch.Testing.Database.DependencyInjection;

/// <summary>Fluent configuration of database testing: the provider, the initializers and the parallelism.</summary>
public interface IDatabaseTestingSetup
{
    IDatabaseTestingSetup SetParallelization(bool allow);

    IDatabaseTestingSetup SetDatabaseSnapshotInitializer<TDatabaseSnapshotInitializer>()
        where TDatabaseSnapshotInitializer : IInitializer;

    IDatabaseTestingSetup SetProvider<TDatabaseTestingProvider>()
        where TDatabaseTestingProvider : IDatabaseTestingProvider, new() =>
        this.SetProvider(new TDatabaseTestingProvider());

    IDatabaseTestingSetup SetProvider(IDatabaseTestingProvider databaseTestingProvider);

    IDatabaseTestingSetup SetEmptySchemaInitializer<TEmptySchemaInitializer>(bool register = true)
        where TEmptySchemaInitializer : class, IInitializer;

    IDatabaseTestingSetup SetTestDataInitializer<TTestDataInitializer>(bool register = true)
        where TTestDataInitializer : class, IInitializer;

    IDatabaseTestingSetup SetSettings(TestDatabaseSettings testDatabaseSettings);

    IDatabaseTestingSetup SetSettings(Func<IServiceProvider, TestDatabaseSettings> testDatabaseSettingsFactory);
}