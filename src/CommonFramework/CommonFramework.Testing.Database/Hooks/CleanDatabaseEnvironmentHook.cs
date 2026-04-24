using CommonFramework.Testing.Database.ConnectionStringManagement;

namespace CommonFramework.Testing.Database.Hooks;

public class CleanDatabaseEnvironmentHook(
    IDatabaseManager databaseManager,
    ITestConnectionStringProvider connectionStringProvider) : ITestEnvironmentHook
{
    public ValueTask Process(CancellationToken ct) => databaseManager.Remove(connectionStringProvider.Actual, ct);
}