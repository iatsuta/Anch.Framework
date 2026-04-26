using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Sqlite;

public class SqliteDatabaseTestingProvider : IDatabaseTestingProvider
{
    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IDatabaseFilePathExtractor, SqliteDatabaseFilePathExtractor>()
            .AddSingleton<ITestDatabaseConnectionStringBuilder, SqliteTestDatabaseConnectionStringBuilder>();
    }
}