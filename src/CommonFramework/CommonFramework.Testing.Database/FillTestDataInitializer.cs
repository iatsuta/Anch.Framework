namespace CommonFramework.Testing.Database;

public class FillTestDataInitializer(
    ISynchronizedInitializer<DatabaseSchemaInitializer> synchronizedInitializer,
    ITestConnectionStringProvider connectionStringProvider,
    IDatabaseChecker databaseChecker,
    IEmptyDatabaseSchemaSeeder emptyDatabaseSchemaSeeder) : IFillTestDataInitializer
{
    public async Task Initialize(CancellationToken cancellationToken) =>

        await synchronizedInitializer.Run(async () =>
        {
            if (!databaseChecker.Exists(connectionStringProvider.FilledSnapshot))
            {
                await emptyDatabaseSchemaSeeder.SeedTestData(cancellationToken);
            }
        });
}

public class RestoreBackupInitializer : IRestoreBackupInitializer
{
    public Task Initialize(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}