using System.Collections.Concurrent;

namespace Anch.Testing.Database.ConnectionStringManagement;

/// <summary>Supplies the connection strings of the test databases by role.</summary>
public class TestConnectionStringProvider(
    ITestConnectionStringFactory testConnectionStringFactory,
    ITestConnectionStringPostfixFactory testConnectionStringPostfixFactory)
    : ITestConnectionStringProvider
{
    private readonly ConcurrentDictionary<TestConnectionStringRole, TestConnectionString> connectionStrings = [];

    public TestConnectionString GetConnectionString(TestConnectionStringRole role) =>
        this.connectionStrings.GetOrAdd(role, _ => testConnectionStringFactory.Create(testConnectionStringPostfixFactory.Create(role)));
}