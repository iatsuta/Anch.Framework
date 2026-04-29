using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public class ServiceProviderPool(ITestEnvironment testEnvironment) : IServiceProviderPool
{
    private int lastIndex;

    private readonly ConcurrentBag<IServiceProvider> pool = [];

    private readonly RootSharedServiceSource rootSharedServiceSource = new();

    private readonly SemaphoreSlim? parallelSemaphoreSlim = testEnvironment.AllowParallelization ? null : new SemaphoreSlim(1, 1);

    public IServiceProvider Get()
    {
        this.parallelSemaphoreSlim?.Wait();

        try
        {
            return this.pool.TryTake(out var serviceProvider)
                ? serviceProvider
                : testEnvironment.BuildServiceProvider(this.CreateServiceCollection());
        }
        catch
        {
            this.parallelSemaphoreSlim?.Release();
            throw;
        }
    }

    public async ValueTask<IServiceProvider> GetAsync(CancellationToken ct)
    {
        if (this.parallelSemaphoreSlim != null)
        {
            await this.parallelSemaphoreSlim.WaitAsync(ct);
        }

        try
        {
            return this.pool.TryTake(out var serviceProvider)
                ? serviceProvider
                : testEnvironment.BuildServiceProvider(this.CreateServiceCollection());
        }
        catch
        {
            this.parallelSemaphoreSlim?.Release();
            throw;
        }
    }

    public void Release(IServiceProvider serviceProvider)
    {
        this.pool.Add(serviceProvider);

        this.parallelSemaphoreSlim?.Release();
    }

    private IServiceCollection CreateServiceCollection() =>
        new ServiceCollection()
            .AddSingleton<ISharedServiceSource>(sp => new SharedServiceSource(this.rootSharedServiceSource, sp))
            .AddSingleton(new ServiceProviderIndex(Interlocked.Increment(ref this.lastIndex) - 1));
}