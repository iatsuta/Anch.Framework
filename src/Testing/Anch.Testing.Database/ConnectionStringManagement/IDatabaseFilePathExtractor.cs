namespace Anch.Testing.Database.ConnectionStringManagement;

public interface IDatabaseFilePathExtractor
{
    string Extract(TestConnectionString connectionString);
}