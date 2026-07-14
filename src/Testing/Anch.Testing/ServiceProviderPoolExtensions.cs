namespace Anch.Testing;

public static class ServiceProviderPoolExtensions
{
    public static async ValueTask<ServiceProviderPoolScope?> TryCreateScopeAsync(this IServiceProviderPool? serviceProviderPool, CancellationToken ct)
    {
        if (serviceProviderPool == null)
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
            if (this.ServiceProvider != null)
            {
                await this.ServiceProvider.RunEnvironmentHooks(EnvironmentHookType.After, ct);

                await serviceProviderPool.ReleaseAsync(this.ServiceProvider, ct);
            }
        }
    }
}