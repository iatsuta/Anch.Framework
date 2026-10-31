using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Sqlite;

/// <summary>Extracts the file path of a SQLite database from a connection string.</summary>
public interface IDatabaseFilePathExtractor
{
    string Extract(TestConnectionStringRole connectionStringRole);
}