using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Sqlite;

public interface IDatabaseFilePathExtractor
{
    string Extract(TestConnectionString connectionString);
}