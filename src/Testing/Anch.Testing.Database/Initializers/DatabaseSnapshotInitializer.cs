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
    TestDatabaseSettings settings) : IInitializer, IAsyncDisposable
{
    private bool disposed;

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
                if (!await databaseManager.Exists(TestConnectionStringRole.EmptySnapshot, cancellationToken))
                {
                    await this.InitializeEmptySchema(cancellationToken);
                }

                if (!await databaseManager.Exists(TestConnectionStringRole.FilledSnapshot, cancellationToken))
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
        await databaseManager.Remove(PoolTestConnectionStringRole.Main, cancellationToken);

        await databaseManager.CreateEmpty(PoolTestConnectionStringRole.Main, cancellationToken);

        await emptySchemaInitializer.Initialize(cancellationToken);

        await databaseManager.Move(PoolTestConnectionStringRole.Main, TestConnectionStringRole.EmptySnapshot, cancellationToken);
    }

    protected virtual async Task InternalInitializeTestData(CancellationToken cancellationToken)
    {
        await databaseManager.Copy(TestConnectionStringRole.EmptySnapshot, PoolTestConnectionStringRole.Main, cancellationToken);

        await testDataInitializer.Initialize(cancellationToken);

        await databaseManager.Move(PoolTestConnectionStringRole.Main, TestConnectionStringRole.FilledSnapshot, cancellationToken);
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
                    await databaseManager.Remove(PoolTestConnectionStringRole.Main, cancellationToken);
                }
                catch (Exception cleanEx)
                {
                    throw new AggregateException(ex, cleanEx);
                }
            }

            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref this.disposed, true))
        {
            return;
        }

        if (settings.InitMode == DatabaseInitMode.RebuildSnapshot)
        {
            await databaseManager.Remove(TestConnectionStringRole.EmptySnapshot, CancellationToken.None);
            await databaseManager.Remove(TestConnectionStringRole.FilledSnapshot, CancellationToken.None);
        }
    }
}