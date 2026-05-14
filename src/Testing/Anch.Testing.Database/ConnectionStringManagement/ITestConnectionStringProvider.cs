namespace Anch.Testing.Database.ConnectionStringManagement;

public interface ITestConnectionStringProvider
{
    TestConnectionString EmptySnapshot { get; }

    TestConnectionString FilledSnapshot { get; }

    TestConnectionString Actual { get; }
}