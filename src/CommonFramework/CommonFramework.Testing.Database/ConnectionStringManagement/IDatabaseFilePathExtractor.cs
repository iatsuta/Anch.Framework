namespace CommonFramework.Testing.Database.ConnectionStringManagement;

public interface IDatabaseFilePathExtractor
{
    string Extract(TestDatabaseConnectionString connectionString);
}