using Anch.Testing.Database.Initializers;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Hooks;

/// <summary>Removes the private database of a container after a test.</summary>
public class CleanDatabaseEnvironmentHook(
    [FromKeyedServices(ITestEnvironment.MainServiceProviderKey)]
    IServiceProvider mainServiceProvider,
    ServiceProviderIndex serviceProviderIndex) : ITestEnvironmentHook
{
    private readonly IDatabaseSnapshotManager databaseSnapshotManager = mainServiceProvider.GetRequiredService<IDatabaseSnapshotManager>();

    public ValueTask Process(CancellationToken ct) => this.databaseSnapshotManager.RemoveRestoredDatabase(serviceProviderIndex, ct);
}