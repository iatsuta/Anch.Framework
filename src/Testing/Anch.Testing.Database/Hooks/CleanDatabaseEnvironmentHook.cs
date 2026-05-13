using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.Initializers;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Hooks;

public class CleanDatabaseEnvironmentHook(
    [FromKeyedServices(IServiceProviderPool.MainServiceProviderKey)]
    IServiceProvider mainServiceProvider,
    IActualTestConnectionStringSource actualTestConnectionStringSource) : ITestEnvironmentHook
{
    private readonly IDatabaseSnapshotManager databaseSnapshotManager = mainServiceProvider.GetRequiredService<IDatabaseSnapshotManager>();

    public ValueTask Process(CancellationToken ct) =>
        this.databaseSnapshotManager.RemoveRestoredDatabase(actualTestConnectionStringSource.ActualConnectionString, ct);
}