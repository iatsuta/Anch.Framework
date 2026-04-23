using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing.Database.Sqlite;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqliteTesting(this IServiceCollection services) =>
        services
            .AddSingleton<ITestConnectionStringProvider, TestConnectionStringProvider>()

            .AddSingleton<IDatabaseFilePathExtractor, SqliteDatabaseFilePathExtractor>()
            .AddSingleton<ITestDatabaseConnectionStringBuilder, SqliteTestDatabaseConnectionStringBuilder>();
}