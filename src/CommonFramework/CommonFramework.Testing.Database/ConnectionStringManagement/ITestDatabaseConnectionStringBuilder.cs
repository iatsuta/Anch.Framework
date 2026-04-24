namespace CommonFramework.Testing.Database.ConnectionStringManagement;

public interface ITestDatabaseConnectionStringBuilder
{
    TestDatabaseConnectionString AddPostfix(string postfix);
}