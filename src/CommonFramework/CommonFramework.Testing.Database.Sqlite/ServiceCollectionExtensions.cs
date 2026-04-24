using CommonFramework.Testing.Database.ConnectionStringManagement;
using CommonFramework.Testing.Database.Hooks;
using CommonFramework.Testing.Database.Initializers;
using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing.Database.Sqlite;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqliteTesting(this IServiceCollection services) =>
        services
            .AddSingleton<ITestConnectionStringProvider, TestConnectionStringProvider>()
            .AddKeyedSingleton<ITestEnvironmentHook, PrepareDatabaseEnvironmentHook>(EnvironmentHookType.Before)
            .AddKeyedSingleton<ITestEnvironmentHook, CleanDatabaseEnvironmentHook>(EnvironmentHookType.After)


            .AddSingleton(typeof(ISynchronizedInitializer<>), typeof(SynchronizedInitializer<>))

            .AddKeyedSingleton<IInitializer, CachedEmptySchemaInitializer>(TestDatabaseInitializer.CachedEmptySchemaKey)
            .AddKeyedSingleton<IInitializer, CachedSharedTestDataInitializer>(TestDatabaseInitializer.CachedSharedTestDataKey)

            .AddSingleton<IDatabaseManager, FileDatabaseManager>()

            .AddSingleton<IDatabaseFilePathExtractor, SqliteDatabaseFilePathExtractor>()
            .AddSingleton<ITestDatabaseConnectionStringBuilder, SqliteTestDatabaseConnectionStringBuilder>();
}