using System.Data.Common;

using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Sqlite;

/// <summary>Extracts the file path of a SQLite database from the test connection string.</summary>
public class SqliteDatabaseFilePathExtractor(ITestConnectionStringProvider testConnectionStringProvider) : IDatabaseFilePathExtractor
{
    public string Extract(TestConnectionStringRole connectionStringRole) => this.Extract(testConnectionStringProvider.GetConnectionString(connectionStringRole));

    private string Extract(TestConnectionString connectionString)
    {
        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString.Value
        };

        if (!builder.TryGetValue("Data Source", out var value))
            throw new InvalidOperationException("Data Source is missing.");

        var dataSource = value.ToString();

        if (string.IsNullOrWhiteSpace(dataSource))
            throw new InvalidOperationException("Data Source is empty.");

        return dataSource;
    }
}