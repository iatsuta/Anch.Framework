using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.XunitEngine;

public class ServiceProviderPool(ITestEnvironment testEnvironment) : IServiceProviderPool
{
    private int lastIndex;

    private readonly ConcurrentBag<IServiceProvider> pool = [];

    private readonly RootSharedServiceSource rootSharedServiceSource = new();

    public IServiceProvider Get() => this.pool.TryTake(out var serviceProvider)
        ? serviceProvider
        : testEnvironment.BuildServiceProvider(this.CreateServiceCollection());

    public void Release(IServiceProvider serviceProvider) => this.pool.Add(serviceProvider);

    private IServiceCollection CreateServiceCollection() =>
        new ServiceCollection()
            .AddSingleton<ISharedServiceSource>(sp => new SharedServiceSource(this.rootSharedServiceSource, sp))
            .AddSingleton(new ServiceProviderIndex(Interlocked.Increment(ref this.lastIndex) - 1));
}