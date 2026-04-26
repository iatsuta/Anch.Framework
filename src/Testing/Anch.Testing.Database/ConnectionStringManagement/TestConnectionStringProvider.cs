namespace Anch.Testing.Database.ConnectionStringManagement;

public class TestConnectionStringProvider(
    ITestDatabaseConnectionStringBuilder testDatabaseConnectionStringBuilder,
    ServiceProviderIndex serviceProviderIndex,
    TestDatabaseSettings settings) : ITestConnectionStringProvider
{
    public TestDatabaseConnectionString EmptySnapshot { get; } = testDatabaseConnectionStringBuilder.AddPostfix("_empty");

    public TestDatabaseConnectionString FilledSnapshot => settings.DefaultConnectionString;

    public TestDatabaseConnectionString Actual { get; } = testDatabaseConnectionStringBuilder.AddPostfix($"_pool_{serviceProviderIndex.Index:D5}");
}