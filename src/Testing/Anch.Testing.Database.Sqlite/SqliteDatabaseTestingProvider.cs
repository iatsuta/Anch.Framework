using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Sqlite;

public class SqliteDatabaseTestingProvider : IDatabaseTestingProvider
{
    public void AddServices(IServiceCollection services)
    {
        services
            .AddSingleton<IDatabaseManager, SqliteDatabaseManager>()
            .AddSingleton<IDatabaseFilePathExtractor, SqliteDatabaseFilePathExtractor>()
            .AddSingleton<ITestConnectionStringFactory, SqliteTestConnectionStringFactory>();
    }
}