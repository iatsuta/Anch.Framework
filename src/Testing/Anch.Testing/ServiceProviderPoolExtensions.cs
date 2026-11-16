namespace Anch.Testing;

/// <summary>Extensions for <see cref="IServiceProviderPool"/>.</summary>
public static class ServiceProviderPoolExtensions
{
    public static async ValueTask<ServiceProviderPoolScope?> TryCreateScopeAsync(this IServiceProviderPool? serviceProviderPool, CancellationToken ct)
    {
        if (serviceProviderPool is null)
        {
            return null;
        }
        else
        {
            return await serviceProviderPool.CreateScopeAsync(ct);
        }
    }

    public static async ValueTask<ServiceProviderPoolScope> CreateScopeAsync(this IServiceProviderPool serviceProviderPool, CancellationToken ct)
    {
        try
        {
            var serviceProvider = await serviceProviderPool.GetAsync(ct);

            await serviceProvider.RunEnvironmentHooks(EnvironmentHookType.Before, ct);

            return new ServiceProviderPoolScope(serviceProviderPool, serviceProvider, null, ct);
        }
        catch (Exception ex)
        {
            return new ServiceProviderPoolScope(serviceProviderPool, null, ex, ct);
        }
    }

    /// <summary>A container borrowed from the pool; disposing the scope returns it (or discards it after a failure).</summary>
    public class ServiceProviderPoolScope(
        IServiceProviderPool serviceProviderPool,
        IServiceProvider? serviceProvider,
        Exception? exception,
        CancellationToken ct) : IAsyncDisposable
    {
        public Exception? Exception { get; } = exception;

        public IServiceProvider? ServiceProvider { get; } = serviceProvider;

        public async ValueTask DisposeAsync()
        {
            if (this.ServiceProvider is not null)
            {
                await this.ServiceProvider.RunEnvironmentHooks(EnvironmentHookType.After, ct);

                await serviceProviderPool.ReleaseAsync(this.ServiceProvider, ct);
            }
        }
    }
}