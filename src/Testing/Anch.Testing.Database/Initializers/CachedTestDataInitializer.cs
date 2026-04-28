using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Initializers;

public class CachedTestDataInitializer(
    ISynchronizedInitializer<CachedTestDataInitializer> synchronizedInitializer,
    ITestConnectionStringProvider connectionStringProvider,
    IDatabaseManager databaseManager,
    [FromKeyedServices(TestDatabaseInitializer.TestDataKey)]
    IInitializer testDataInitializer,
    TestDatabaseSettings settings) : IInitializer
{
    public Task Initialize(CancellationToken cancellationToken) =>

        synchronizedInitializer.Run(async () =>
        {
            switch (settings.InitMode)
            {
                case DatabaseInitMode.RebuildSnapshot:
                {
                    await this.InternalInitialize(cancellationToken);
                    break;
                }

                case DatabaseInitMode.ReuseSnapshot:
                {
                    if (!await databaseManager.Exists(connectionStringProvider.FilledSnapshot, cancellationToken))
                    {
                        await this.InternalInitialize(cancellationToken);
                    }

                    break;
                }
            }
        });


    private async Task InternalInitialize(CancellationToken cancellationToken)
    {
        try
        {
            await databaseManager.Copy(connectionStringProvider.EmptySnapshot, connectionStringProvider.Actual, true, cancellationToken);

            await testDataInitializer.Initialize(cancellationToken);

            await databaseManager.Move(connectionStringProvider.Actual, connectionStringProvider.FilledSnapshot, true, cancellationToken);
        }
        catch (Exception createSchemaEx)
        {
            if (settings.RemoveDatabaseOnFailure)
            {
                try
                {
                    await databaseManager.Remove(connectionStringProvider.Actual, cancellationToken);
                    await databaseManager.Remove(connectionStringProvider.FilledSnapshot, cancellationToken);
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