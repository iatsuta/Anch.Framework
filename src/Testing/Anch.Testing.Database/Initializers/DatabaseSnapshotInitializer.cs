using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Initializers;

public class DatabaseSnapshotInitializer(
    [FromKeyedServices(TestDatabaseInitializer.EmptySchemaKey)]
    IEnumerable<IInitializer> emptySchemaInitializers,
    [FromKeyedServices(TestDatabaseInitializer.TestDataKey)]
    IEnumerable<IInitializer> testDataInitializers,
    IDatabaseManager databaseManager,
    TestDatabaseSettings settings) : IInitializer, IAsyncDisposable
{
    private bool disposed;

    public async Task Initialize(CancellationToken ct)
    {
        switch (settings.InitMode)
        {
            case DatabaseInitMode.RebuildSnapshot:
                {
                    await this.InitializeEmptySchema(ct);
                    await this.InitializeTestData(ct);

                    break;
                }

            case DatabaseInitMode.ReuseSnapshot:
                {
                    if (!await databaseManager.Exists(TestConnectionStringRole.EmptySnapshot, ct))
                    {
                        await this.InitializeEmptySchema(ct);
                    }

                    if (!await databaseManager.Exists(TestConnectionStringRole.FilledSnapshot, ct))
                    {
                        await this.InitializeTestData(ct);
                    }

                    break;
                }

            case DatabaseInitMode.External:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(settings.InitMode), settings.InitMode, null);
        }
    }

    protected virtual async Task InternalInitializeEmptySchema(CancellationToken ct)
    {
        await databaseManager.Remove(PoolTestConnectionStringRole.Main, ct);

        await databaseManager.CreateEmpty(PoolTestConnectionStringRole.Main, ct);

        foreach (var emptySchemaInitializer in emptySchemaInitializers)
        {
            await emptySchemaInitializer.Initialize(ct);
        }

        await databaseManager.Move(PoolTestConnectionStringRole.Main, TestConnectionStringRole.EmptySnapshot, ct);
    }

    protected virtual async Task InternalInitializeTestData(CancellationToken ct)
    {
        await databaseManager.Copy(TestConnectionStringRole.EmptySnapshot, PoolTestConnectionStringRole.Main, ct);

        foreach (var testDataInitializer in testDataInitializers)
        {
            await testDataInitializer.Initialize(ct);
        }

        await databaseManager.Move(PoolTestConnectionStringRole.Main, TestConnectionStringRole.FilledSnapshot, ct);
    }

    private Task InitializeEmptySchema(CancellationToken ct) =>

        this.SafeInitialize(() => this.InternalInitializeEmptySchema(ct), ct);

    private Task InitializeTestData(CancellationToken ct) =>

        this.SafeInitialize(() => this.InternalInitializeTestData(ct), ct);

    private async Task SafeInitialize(Func<Task> initAction, CancellationToken ct)
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
                    await databaseManager.Remove(PoolTestConnectionStringRole.Main, ct);
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