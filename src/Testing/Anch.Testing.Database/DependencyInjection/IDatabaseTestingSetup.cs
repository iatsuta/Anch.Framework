using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.DependencyInjection;

public interface IDatabaseTestingSetup
{
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

    IDatabaseTestingSetup RebindActualConnection<T>(Func<TestDatabaseConnectionString, T> rebindFunc)
        where T : class;
}