namespace Anch.Testing.Database.ConnectionStringManagement;

/// <summary>Creates the connection string of a test database for a role.</summary>
public interface ITestConnectionStringFactory
{
    TestConnectionString Create(string postfix);
}