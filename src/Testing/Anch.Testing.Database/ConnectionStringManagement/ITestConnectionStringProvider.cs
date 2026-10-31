namespace Anch.Testing.Database.ConnectionStringManagement;

/// <summary>Supplies the connection strings of the test databases by role.</summary>
public interface ITestConnectionStringProvider
{
    TestConnectionString GetConnectionString(TestConnectionStringRole role);
}