namespace Anch.Testing.Database.ConnectionStringManagement;

public interface ITestConnectionStringFactory
{
    TestConnectionString Create(string postfix);
}