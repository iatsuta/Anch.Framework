namespace CommonFramework.Testing.Database;

public class SqliteDatabaseInitializer(
    ISynchronizedInitializer<SqliteDatabaseInitializer> synchronizedInitializer,
    ITestConnectionStringProvider testConnectionStringProvider,
    IDatabaseSchemaGenerator databaseSchemaGenerator) : IDatabaseSchemaInitializer
{
    public async Task Initialize(CancellationToken cancellationToken) =>

        await synchronizedInitializer.Run(async () =>
        {
            var schemaConnectionString = testConnectionStringProvider.CreateWithPostfix("_Empty");

            await databaseSchemaGenerator.Generate(schemaConnectionString, cancellationToken);
        });
}