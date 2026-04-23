using System.Collections.Concurrent;

using CommonFramework.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing.XunitEngine;

public class ServiceProviderPool(ITestEnvironment testEnvironment) : IServiceProviderPool
{
    private int lastIndex;

    private readonly ConcurrentBag<IServiceProvider> pool = [];

    private readonly IServiceProviderSynchronizationContext serviceProviderSynchronizationLock =
        new ServiceProviderSynchronizationContext(new AsyncLockerProvider());

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
        foreach (var hook in serviceProvider.GetKeyedServices<ITestEnvironmentHook>(EnvironmentHookType.After)
                     .Reverse())
        {
            await hook.Process(ct);
        }

        this.Release(serviceProvider);
    }

    private IServiceProvider Get() => this.pool.TryTake(out var serviceProvider)
        ? serviceProvider
        : testEnvironment.BuildServiceProvider(this.CreateServiceCollection());

    private void Release(IServiceProvider serviceProvider) => this.pool.Add(serviceProvider);

    private IServiceCollection CreateServiceCollection()
    {
        return new ServiceCollection()
            .AddSingleton(this.serviceProviderSynchronizationLock)
            .AddSingleton(new ServiceProviderIndex(Interlocked.Increment(ref this.lastIndex) - 1));
    }
}