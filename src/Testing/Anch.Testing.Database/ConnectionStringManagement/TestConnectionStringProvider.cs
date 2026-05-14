namespace Anch.Testing.Database.ConnectionStringManagement;

public class TestConnectionStringProvider(
    ITestConnectionStringFactory testConnectionStringFactory,
    IActualTestConnectionStringSource actualTestConnectionStringSource)
    : ITestConnectionStringProvider
{
    public TestConnectionString EmptySnapshot { get; } = testConnectionStringFactory.Create("_empty");

    public TestConnectionString FilledSnapshot { get; } = testConnectionStringFactory.Create("_filled");

    public TestConnectionString Actual { get; } = actualTestConnectionStringSource.ActualConnectionString;
}