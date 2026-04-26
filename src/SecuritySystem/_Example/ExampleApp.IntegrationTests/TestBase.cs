using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.IntegrationTests;

public abstract class TestBase(IServiceProvider rootServiceProvider) : IAsyncLifetime
{
    protected ITestingEvaluator<TService> GetEvaluator<TService>() => rootServiceProvider.GetRequiredService<ITestingEvaluator<TService>>();

    protected RootAuthManager AuthManager => field ??= rootServiceProvider.GetRequiredService<RootAuthManager>();

    protected virtual ValueTask InitializeAsync(CancellationToken ct) => ValueTask.CompletedTask;

    ValueTask IAsyncLifetime.InitializeAsync() => this.InitializeAsync(TestContext.Current.CancellationToken);

    ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;
}