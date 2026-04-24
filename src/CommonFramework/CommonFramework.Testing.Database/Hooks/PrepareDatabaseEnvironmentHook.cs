using CommonFramework.Testing.Database.ConnectionStringManagement;
using CommonFramework.Testing.Database.Initializers;
using CommonFramework.Testing.XunitEngine;

namespace CommonFramework.Testing.Database.Hooks;

public class PrepareDatabaseEnvironmentHook(
    ISharedServiceSource sharedServiceSource,
    IDatabaseManager databaseManager,
    ITestConnectionStringProvider connectionStringProvider) : ITestEnvironmentHook
{
    private readonly IInitializer emptySchemaInitializer = sharedServiceSource.GetSharedService<IInitializer>(TestDatabaseInitializer.CachedEmptySchemaKey);

    private readonly IInitializer sharedTestDataInitializer = sharedServiceSource.GetSharedService<IInitializer>(TestDatabaseInitializer.CachedSharedTestDataKey);

    public async ValueTask Process(CancellationToken ct)
    {
        await this.emptySchemaInitializer.Initialize(ct);

        await this.sharedTestDataInitializer.Initialize(ct);

        await databaseManager.Copy(connectionStringProvider.FilledSnapshot, connectionStringProvider.Actual, true, ct);
    }
}