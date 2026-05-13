using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Initializers;

public interface IDatabaseSnapshotManager
{
    ValueTask RestoreDatabaseSnapshot(TestConnectionString actualConnectionString, CancellationToken ct);

    ValueTask RemoveRestoredDatabase(TestConnectionString actualConnectionString, CancellationToken ct);
}