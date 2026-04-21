using HierarchicalExpand.IntegrationTests.Environment;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.IntegrationTests;

public abstract class TestBase(IServiceProvider rootServiceProvider) : IAsyncLifetime
{
    protected ScopeEvaluator ScopeEvaluator => field ??= rootServiceProvider.GetRequiredService<ScopeEvaluator>();

    protected virtual async ValueTask InitializeAsync(CancellationToken ct)
    {
        {
            await using var scope = rootServiceProvider.CreateAsyncScope();

            await scope.ServiceProvider.GetRequiredService<IDbSchemaInitializer>().Initialize(ct);
        }

        {
            await using var scope = rootServiceProvider.CreateAsyncScope();

            await scope.ServiceProvider.GetRequiredService<TestDataInitializer>().Initialize(ct);
        }
    }


    ValueTask IAsyncLifetime.InitializeAsync() => this.InitializeAsync(TestContext.Current.CancellationToken);

    ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;
}