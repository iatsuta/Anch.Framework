namespace Anch.Testing.Database.ConnectionStringManagement;

public class TestConnectionStringProvider(
    ITestConnectionStringBuilder testDatabaseConnectionStringBuilder,
    IActualTestConnectionStringSource actualTestConnectionStringSource)
    : ITestConnectionStringProvider
{
    public TestConnectionString EmptySnapshot { get; } = testDatabaseConnectionStringBuilder.AddPostfix("_empty");

    public TestConnectionString FilledSnapshot { get; } = testDatabaseConnectionStringBuilder.AddPostfix("_filled");

    public TestConnectionString Actual { get; } = actualTestConnectionStringSource.ActualConnectionString;
}