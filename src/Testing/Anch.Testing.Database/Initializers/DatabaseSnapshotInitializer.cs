using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Initializers;

public class DatabaseSnapshotInitializer(
    [FromKeyedServices(TestDatabaseInitializer.EmptySchemaKey)] IInitializer emptySchemaInitializer,
    [FromKeyedServices(TestDatabaseInitializer.TestDataKey)] IInitializer testDataInitializer,
    IDatabaseManager databaseManager,
    TestDatabaseSettings settings,
    ITestConnectionStringProvider connectionStringProvider) : IInitializer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        switch (settings.InitMode)
        {
            case DatabaseInitMode.RebuildSnapshot:
            {
                await this.InitializeSchema(cancellationToken);
                await this.InitializeTestData(cancellationToken);

                break;
            }

            case DatabaseInitMode.ReuseSnapshot:
            {
                if (!await databaseManager.Exists(connectionStringProvider.EmptySnapshot, cancellationToken))
                {
                    await this.InitializeSchema(cancellationToken);
                }

                if (!await databaseManager.Exists(connectionStringProvider.FilledSnapshot, cancellationToken))
                {
                    await this.InitializeTestData(cancellationToken);
                }

                break;
            }

            case DatabaseInitMode.External:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(settings.InitMode), settings.InitMode, null);
        }
    }


    protected virtual async ValueTask InitializeSchema(CancellationToken cancellationToken)
    {
        try
        {
            await databaseManager.Remove(connectionStringProvider.Main, cancellationToken);

            await emptySchemaInitializer.Initialize(cancellationToken);

            await databaseManager.Move(connectionStringProvider.Main, connectionStringProvider.EmptySnapshot, true, cancellationToken);
        }
        catch (Exception ex)
        {
            if (settings.RemoveDatabaseOnFailure)
            {
                try
                {
                    await databaseManager.Remove(connectionStringProvider.Main, cancellationToken);
                }
                catch (Exception cleanEx)
                {
                    throw new AggregateException(ex, cleanEx);
                }
            }

            throw;
        }
    }
    protected virtual async ValueTask InitializeTestData(CancellationToken cancellationToken)
    {
        try
        {
            await databaseManager.Copy(connectionStringProvider.EmptySnapshot, connectionStringProvider.Main, true, cancellationToken);

            await testDataInitializer.Initialize(cancellationToken);

            await databaseManager.Move(connectionStringProvider.Main, connectionStringProvider.FilledSnapshot, true, cancellationToken);
        }
        catch (Exception ex)
        {
            if (settings.RemoveDatabaseOnFailure)
            {
                try
                {
                    await databaseManager.Remove(connectionStringProvider.Main, cancellationToken);
                }
                catch (Exception cleanEx)
                {
                    throw new AggregateException(ex, cleanEx);
                }
            }

            throw;
        }
    }
}