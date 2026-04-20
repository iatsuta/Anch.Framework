using CommonFramework;

using ExampleApp.Infrastructure.Services;

using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.IntegrationTests;

public abstract class TestBase(IServiceProvider rootServiceProvider) : IAsyncLifetime
{
    protected IServiceProvider RootServiceProvider { get; } = rootServiceProvider;

    protected ITestingEvaluator<TService> GetEvaluator<TService>() => this.RootServiceProvider.GetRequiredService<ITestingEvaluator<TService>>();


    protected RootAuthManager AuthManager => this.RootServiceProvider.GetRequiredService<RootAuthManager>();

    ValueTask IAsyncLifetime.InitializeAsync() => this.InitializeAsync(TestContext.Current.CancellationToken);

    protected virtual async ValueTask InitializeAsync(CancellationToken ct)
    {
        this.RootServiceProvider.GetRequiredService<RootImpersonateServiceState>().Reset();

        await this.RootServiceProvider.GetRequiredKeyedService<IInitializer>(RootAppInitializer.Key).Initialize(ct);
    }

	public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
}