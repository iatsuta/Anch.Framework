namespace Anch.Testing.Database.Initializers;

/// <summary>Creates snapshots of the reference database and restores them into the private databases of the containers.</summary>
public interface IDatabaseSnapshotManager
{
    ValueTask RestoreDatabaseSnapshot(ServiceProviderIndex serviceProviderIndex, CancellationToken ct);

    ValueTask RemoveRestoredDatabase(ServiceProviderIndex serviceProviderIndex, CancellationToken ct);
}