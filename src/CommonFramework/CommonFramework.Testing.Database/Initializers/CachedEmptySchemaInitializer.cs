using CommonFramework.Testing.Database.ConnectionStringManagement;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing.Database.Initializers;

public class CachedEmptySchemaInitializer(
    ISynchronizedInitializer<CachedEmptySchemaInitializer> synchronizedInitializer,
    ITestConnectionStringProvider connectionStringProvider,
    IDatabaseManager databaseManager,
    [FromKeyedServices(TestDatabaseInitializer.EmptySchemaKey)]
    IInitializer emptySchemaInitializer,
    TestDatabaseSettings settings) : IInitializer
{
    public Task Initialize(CancellationToken cancellationToken) =>

        synchronizedInitializer.Run(async () =>
        {
            switch (settings.InitMode)
            {
                case DatabaseInitMode.RebuildSnapshot:
                {
                    await this.InternalInitialize(true, cancellationToken);
                    break;
                }

                case DatabaseInitMode.ReuseSnapshot:
                {
                    if (!await databaseManager.Exists(connectionStringProvider.EmptySnapshot, cancellationToken))
                    {
                        await this.InternalInitialize(false, cancellationToken);
                    }

                    break;
                }
            }
        });

    private async Task InternalInitialize(bool force, CancellationToken cancellationToken)
    {
        try
        {
            await emptySchemaInitializer.Initialize(cancellationToken);

            await databaseManager.Move(connectionStringProvider.Actual, connectionStringProvider.EmptySnapshot, force, cancellationToken);
        }
        catch (Exception createSchemaEx)
        {
            if (settings.RemoveDatabaseOnFailure)
            {
                try
                {
                    await databaseManager.Remove(connectionStringProvider.Actual, cancellationToken);
                    await databaseManager.Remove(settings.DefaultConnectionString, cancellationToken);
                }
                catch (Exception cleanEx)
                {
                    throw new AggregateException(createSchemaEx, cleanEx);
                }
            }

            throw;
        }
    }
}