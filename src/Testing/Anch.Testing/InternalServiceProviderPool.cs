using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public class InternalServiceProviderPool(ITestEnvironment testEnvironment, IServiceProvider mainServiceProvider, MainServiceProviderSettings settings)
    : IServiceProviderPool
{
    private int lastIndex;

    private readonly ConcurrentBag<IServiceProvider> pool = settings.ReturnToServicePool ? [mainServiceProvider] : [];

    private readonly SemaphoreSlim? parallelSemaphoreSlim = settings.AllowParallelization ? null : new SemaphoreSlim(1, 1);

    public async ValueTask<IServiceProvider> GetAsync(CancellationToken ct)
    {
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
                .AddKeyedSingleton(mainServiceProvider, IServiceProviderPool.MainServiceProviderKey)
                .AddSingleton(serviceProviderIndex);

            try
            {
                return testEnvironment.BuildServiceProvider(services, serviceProviderIndex);
            }
            catch (Exception)
            {
                this.parallelSemaphoreSlim?.Release();

                throw;
            }
        }
    }

    public async ValueTask ReleaseAsync(IServiceProvider serviceProvider, CancellationToken ct)
    {
        this.pool.Add(serviceProvider);

        this.parallelSemaphoreSlim?.Release();
    }
}