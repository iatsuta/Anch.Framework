using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Hooks;

public class CleanDatabaseEnvironmentHook(
    IDatabaseManager databaseManager,
    ITestConnectionStringProvider connectionStringProvider,
    TestDatabaseSettings databaseSettings) : ITestEnvironmentHook
{
    public ValueTask Process(CancellationToken ct)
    {
        if (databaseSettings.InitMode == DatabaseInitMode.External)
        {
            return ValueTask.CompletedTask;
        }
        else
        {
            return databaseManager.Remove(connectionStringProvider.Actual, ct);
        }
    }
}