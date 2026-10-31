using System.Data.Common;

using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Sqlite;

/// <summary>Creates SQLite connection strings for the test databases.</summary>
public class SqliteTestConnectionStringFactory(TestDatabaseSettings databaseSettings) : ITestConnectionStringFactory
{
    public TestConnectionString Create(string postfix)
    {
        var builder = new DbConnectionStringBuilder { ConnectionString = databaseSettings.RawConnectionString.Value };

        if (!string.IsNullOrWhiteSpace(postfix))
        {
            var dataSource = builder["Data Source"].ToString();

            if (string.IsNullOrWhiteSpace(dataSource))
                throw new InvalidOperationException("Data Source is missing in connection string.");

            var directory = Path.GetDirectoryName(dataSource);
            var fileName = Path.GetFileNameWithoutExtension(dataSource);
            var extension = Path.GetExtension(dataSource);

            var newFileName = $"{fileName}_{postfix}{extension}";
            var newDataSource = directory is null ? newFileName : Path.Combine(directory, newFileName);

            builder["Data Source"] = newDataSource;
        }

        return new TestConnectionString(builder.ConnectionString);
    }
}