using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Initializers;

public class DatabaseSnapshotManager(
    IDatabaseManager databaseManager,
    ITestConnectionStringProvider testConnectionStringProvider,
    IActualConnectionStringResolver actualConnectionStringResolver) : IDatabaseSnapshotManager
{
    public async ValueTask RestoreDatabaseSnapshot(ServiceProviderIndex serviceProviderIndex, CancellationToken ct)
    {
        var actualConnectionString = actualConnectionStringResolver.GetActualConnectionString(serviceProviderIndex);

        await databaseManager.Copy(testConnectionStringProvider.FilledSnapshot, actualConnectionString, true, ct);
    }

    public async ValueTask RemoveRestoredDatabase(ServiceProviderIndex serviceProviderIndex, CancellationToken ct)
    {
        var actualConnectionString = actualConnectionStringResolver.GetActualConnectionString(serviceProviderIndex);

        await databaseManager.Remove(actualConnectionString, ct);
    }
}