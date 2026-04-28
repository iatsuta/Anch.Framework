using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.DependencyInjection;

public interface IDatabaseTestingSetup
{
    IDatabaseTestingSetup SetProvider<TDatabaseTestingProvider>()
        where TDatabaseTestingProvider : IDatabaseTestingProvider, new();

    IDatabaseTestingSetup SetEmptySchemaInitializer<TEmptySchemaInitializer>(bool register = true)
        where TEmptySchemaInitializer : class, IInitializer;

    IDatabaseTestingSetup SetSharedTestDataInitializer<TSharedTestDataInitializer>(bool register = true)
        where TSharedTestDataInitializer : class, IInitializer;

    IDatabaseTestingSetup SetSettings(TestDatabaseSettings testDatabaseSettings);

    IDatabaseTestingSetup RebindActualConnection<T>(Func<TestDatabaseConnectionString, T> rebindFunc)
        where T : class;
}