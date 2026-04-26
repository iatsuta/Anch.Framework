namespace Anch.Testing.Database.ConnectionStringManagement;

public interface ITestDatabaseConnectionStringBuilder
{
    TestDatabaseConnectionString AddPostfix(string postfix);
}