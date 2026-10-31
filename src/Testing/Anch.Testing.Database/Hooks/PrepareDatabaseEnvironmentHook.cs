using Anch.Testing.Database.Initializers;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Hooks;

/// <summary>Restores the private database of a container from the filled snapshot before a test.</summary>
public class PrepareDatabaseEnvironmentHook(
    [FromKeyedServices(ITestEnvironment.MainServiceProviderKey)]
    IServiceProvider mainServiceProvider,
    ServiceProviderIndex serviceProviderIndex)
    : ITestEnvironmentHook
{
    private readonly IDatabaseSnapshotManager databaseSnapshotManager = mainServiceProvider.GetRequiredService<IDatabaseSnapshotManager>();

    public ValueTask Process(CancellationToken ct) => this.databaseSnapshotManager.RestoreDatabaseSnapshot(serviceProviderIndex, ct);
}