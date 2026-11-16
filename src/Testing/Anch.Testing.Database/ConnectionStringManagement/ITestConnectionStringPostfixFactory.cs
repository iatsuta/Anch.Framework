namespace Anch.Testing.Database.ConnectionStringManagement;

/// <summary>Creates the suffix that makes the connection string of each pooled database unique.</summary>
public interface ITestConnectionStringPostfixFactory
{
    string Create(TestConnectionStringRole testConnectionStringRole);
}