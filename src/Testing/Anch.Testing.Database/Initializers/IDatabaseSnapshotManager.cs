namespace Anch.Testing.Database.Initializers;

public interface IDatabaseSnapshotManager
{
    ValueTask RestoreDatabaseSnapshot(ServiceProviderIndex serviceProviderIndex, CancellationToken ct);

    ValueTask RemoveRestoredDatabase(ServiceProviderIndex serviceProviderIndex, CancellationToken ct);
}