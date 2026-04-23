namespace CommonFramework.Testing.Database;

public interface ITestDatabaseConnectionStringBuilder
{
    TestDatabaseConnectionString AddPostfix(string postfix);
}