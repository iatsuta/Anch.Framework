namespace Anch.Testing.Database.ConnectionStringManagement;

public interface ITestConnectionStringProvider
{
    TestDatabaseConnectionString EmptySnapshot { get; }

    TestDatabaseConnectionString FilledSnapshot { get; }

    TestDatabaseConnectionString Actual { get; }
}