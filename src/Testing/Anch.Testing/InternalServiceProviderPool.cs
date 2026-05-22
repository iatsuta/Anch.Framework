using System.Collections.Concurrent;

using Anch.Core;
using Anch.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public class InternalServiceProviderPool(
    ITestEnvironment testEnvironment,
    IServiceProvider mainServiceProvider,
    IParallelizationSettings parallelizationSettings,
    bool returnMainServiceProviderToPool)
    : IServiceProviderPool
{
    private bool disposed;

    private int lastIndex;

    private readonly ConcurrentBag<IServiceProvider> pool = returnMainServiceProviderToPool ? [mainServiceProvider] : [];

    private readonly SemaphoreSlim? parallelSemaphoreSlim = parallelizationSettings.AllowParallelization ? null : new SemaphoreSlim(1, 1);

    private readonly AsyncOnce mainInitialization = new(async ct =>
    {
        foreach (var initializer in mainServiceProvider.GetKeyedServices<IInitializer>(ITestEnvironment.MainServiceProviderKey))
        {
            await initializer.Initialize(ct);
        }
    });

    public async ValueTask<IServiceProvider> GetAsync(CancellationToken ct)
    {
        await this.mainInitialization.EnsureExecutedAsync(ct);

        if (this.parallelSemaphoreSlim != null)
        {
            await this.parallelSemaphoreSlim.WaitAsync(ct);
        }

        if (this.pool.TryTake(out var serviceProvider))
        {
            return serviceProvider;
        }
        else
        {
            var serviceProviderIndex = new ServiceProviderIndex(Interlocked.Increment(ref this.lastIndex) - 1);

            var services = new ServiceCollection()
                .AddKeyedSingleton(ITestEnvironment.MainServiceProviderKey, mainServiceProvider)
                .AddSingleton(serviceProviderIndex);

            try
            {
                return testEnvironment.BuildServiceProvider(services, new PooledServiceProviderBuildContext(serviceProviderIndex, mainServiceProvider));
            }
            catch (Exception)
            {
                this.parallelSemaphoreSlim?.Release();

                throw;
            }
        }
    }

    public ValueTask ReleaseAsync(IServiceProvider serviceProvider, CancellationToken ct)
    {
        this.pool.Add(serviceProvider);

        this.parallelSemaphoreSlim?.Release();

        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref this.disposed, true))
        {
            return;
        }

        using (this.parallelSemaphoreSlim)
        {
            foreach (var serviceProvider in this.pool.Except([mainServiceProvider]).Concat([mainServiceProvider]))
            {
                if (serviceProvider is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
            }
        }
    }
}