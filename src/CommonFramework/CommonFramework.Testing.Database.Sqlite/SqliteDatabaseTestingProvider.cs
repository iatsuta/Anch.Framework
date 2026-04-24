using CommonFramework.Testing.Database.ConnectionStringManagement;
using CommonFramework.Testing.Database.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing.Database.Sqlite;

public class SqliteDatabaseTestingProvider : IDatabaseTestingProvider
{
    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IDatabaseFilePathExtractor, SqliteDatabaseFilePathExtractor>()
            .AddSingleton<ITestDatabaseConnectionStringBuilder, SqliteTestDatabaseConnectionStringBuilder>();
    }
}