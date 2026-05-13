using Anch.Testing.Database.Initializers;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Hooks;

public class CleanDatabaseEnvironmentHook(
    [FromKeyedServices(IServiceProviderPool.MainServiceProviderKey)] IServiceProvider mainServiceProvider,
    ServiceProviderIndex serviceProviderIndex) : ITestEnvironmentHook
{
    private readonly IDatabaseSnapshotManager databaseSnapshotManager = mainServiceProvider.GetRequiredService<IDatabaseSnapshotManager>();

    public ValueTask Process(CancellationToken ct) => this.databaseSnapshotManager.RemoveRestoredDatabase(serviceProviderIndex, ct);
}