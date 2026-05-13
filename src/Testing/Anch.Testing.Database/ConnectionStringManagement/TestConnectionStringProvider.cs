namespace Anch.Testing.Database.ConnectionStringManagement;

public class TestConnectionStringProvider(ITestConnectionStringBuilder testDatabaseConnectionStringBuilder, TestDatabaseSettings settings)
    : ITestConnectionStringProvider
{
    public TestConnectionString EmptySnapshot { get; } = testDatabaseConnectionStringBuilder.AddPostfix("_empty");

    public TestConnectionString FilledSnapshot { get; } = testDatabaseConnectionStringBuilder.AddPostfix("_filled");

    public TestConnectionString Main { get; } = settings.MainConnectionString;
}