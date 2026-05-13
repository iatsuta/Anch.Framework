using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Initializers;

public class DatabaseSnapshotManager(IDatabaseManager databaseManager, ITestConnectionStringProvider testConnectionStringProvider) : IDatabaseSnapshotManager
{
    public ValueTask RestoreDatabaseSnapshot(TestConnectionString actualConnectionString, CancellationToken ct) =>
        databaseManager.Copy(testConnectionStringProvider.FilledSnapshot, actualConnectionString, true, ct);

    public ValueTask RemoveRestoredDatabase(TestConnectionString actualConnectionString, CancellationToken ct) =>
        databaseManager.Remove(actualConnectionString, ct);
}