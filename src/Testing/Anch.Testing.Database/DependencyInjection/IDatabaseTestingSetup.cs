using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.DependencyInjection;

public interface IDatabaseTestingSetup
{
    IDatabaseTestingSetup SetProvider<TDatabaseTestingProvider>()
        where TDatabaseTestingProvider : IDatabaseTestingProvider, new();

    IDatabaseTestingSetup SetEmptySchemaInitializer<TEmptySchemaInitializer>()
        where TEmptySchemaInitializer : IInitializer;

    IDatabaseTestingSetup SetSharedTestDataInitializer<TSharedTestDataInitializer>()
        where TSharedTestDataInitializer : IInitializer;

    IDatabaseTestingSetup SetSettings(TestDatabaseSettings testDatabaseSettings);

    IDatabaseTestingSetup RebindActualConnection<T>(Func<TestDatabaseConnectionString, T> rebindFunc)
        where T : class;
}