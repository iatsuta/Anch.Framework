using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing.XunitEngine;

public class ServiceProviderPool(ITestEnvironment testEnvironment) : IServiceProviderPool
{
    private readonly ConcurrentBag<IServiceProvider> pool = [];

    public async ValueTask<IServiceProvider> GetAsync(CancellationToken ct)
    {
        var serviceProvider = this.Get();

        foreach (var hook in serviceProvider.GetKeyedServices<ITestEnvironmentHook>(EnvironmentHookType.Before))
        {
            await hook.Process(ct);
        }

        return serviceProvider;
    }

    public async ValueTask ReleaseAsync(IServiceProvider serviceProvider, CancellationToken ct)
    {
        foreach (var hook in serviceProvider.GetKeyedServices<ITestEnvironmentHook>(EnvironmentHookType.After).Reverse())
        {
            await hook.Process(ct);
        }

        this.Release(serviceProvider);
    }

    private IServiceProvider Get() =>
        this.pool.TryTake(out var serviceProvider)
            ? serviceProvider
            : testEnvironment.BuildServiceProvider(new ServiceCollection());

    private void Release(IServiceProvider serviceProvider) => this.pool.Add(serviceProvider);
}