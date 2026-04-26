using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Hooks;

public class CleanDatabaseEnvironmentHook(
    IDatabaseManager databaseManager,
    ITestConnectionStringProvider connectionStringProvider) : ITestEnvironmentHook
{
    public ValueTask Process(CancellationToken ct) => databaseManager.Remove(connectionStringProvider.Actual, ct);
}