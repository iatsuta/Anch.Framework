namespace Anch.Testing.Database.Initializers;

public class DatabaseSnapshotManager : IDatabaseSnapshotManager
{
    public ValueTask RestoreDatabaseSnapshot(ServiceProviderIndex serviceProviderIndex, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public ValueTask RemoveRestoredDatabase(ServiceProviderIndex serviceProviderIndex, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}