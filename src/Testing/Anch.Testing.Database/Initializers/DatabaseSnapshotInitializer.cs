using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Initializers;

public class DatabaseSnapshotInitializer(
    [FromKeyedServices(TestDatabaseInitializer.EmptySchemaKey)]
    IInitializer emptySchemaInitializer,
    [FromKeyedServices(TestDatabaseInitializer.TestDataKey)]
    IInitializer testDataInitializer,
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
                await this.InitializeEmptySchema(cancellationToken);
                await this.InitializeTestData(cancellationToken);

                break;
            }

            case DatabaseInitMode.ReuseSnapshot:
            {
                if (!await databaseManager.Exists(connectionStringProvider.EmptySnapshot, cancellationToken))
                {
                    await this.InitializeEmptySchema(cancellationToken);
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

    protected virtual async Task InternalInitializeEmptySchema(CancellationToken cancellationToken)
    {
        await databaseManager.Remove(connectionStringProvider.Actual, cancellationToken);

        await emptySchemaInitializer.Initialize(cancellationToken);

        await databaseManager.Move(connectionStringProvider.Actual, connectionStringProvider.EmptySnapshot, true, cancellationToken);
    }

    protected virtual async Task InternalInitializeTestData(CancellationToken cancellationToken)
    {
        await databaseManager.Copy(connectionStringProvider.EmptySnapshot, connectionStringProvider.Actual, true, cancellationToken);

        await testDataInitializer.Initialize(cancellationToken);

        await databaseManager.Move(connectionStringProvider.Actual, connectionStringProvider.FilledSnapshot, true, cancellationToken);
    }

    private Task InitializeEmptySchema(CancellationToken cancellationToken) =>

        this.SafeInitialize(() => this.InternalInitializeEmptySchema(cancellationToken), cancellationToken);

    private Task InitializeTestData(CancellationToken cancellationToken) =>

        this.SafeInitialize(() => this.InternalInitializeTestData(cancellationToken), cancellationToken);

    private async Task SafeInitialize(Func<Task> initAction, CancellationToken cancellationToken)
    {
        try
        {
            await initAction();
        }
        catch (Exception ex)
        {
            if (settings.RemoveDatabaseOnFailure)
            {
                try
                {
                    await databaseManager.Remove(connectionStringProvider.Actual, cancellationToken);
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