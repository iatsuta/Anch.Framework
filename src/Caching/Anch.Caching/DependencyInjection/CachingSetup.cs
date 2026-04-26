using Anch.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anch.Caching.DependencyInjection;

public class CachingSetup : ICachingSetup, IServiceInitializer
{
    private Type? cacheProviderType;

    public ICachingSetup SetCacheProvider<TCacheProvider>()
        where TCacheProvider : ICacheProvider
    {
        this.cacheProviderType = typeof(TCacheProvider);

        return this;
    }

    public void Initialize(IServiceCollection services)
    {
        if (!services.AlreadyInitialized<ICacheProvider>() || this.cacheProviderType != null)
        {
            services.TryAddSingleton(typeof(ICache<,>), typeof(CacheProxy<,>));
            services.ReplaceSingleton(typeof(ICacheProvider), this.cacheProviderType ?? typeof(CacheProvider));
        }
    }
}