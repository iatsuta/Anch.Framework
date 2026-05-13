namespace Anch.Testing.Database.ConnectionStringManagement;

public interface ITestConnectionStringBuilder
{
    TestConnectionString AddPostfix(string postfix);
}