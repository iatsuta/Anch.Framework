using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommonFramework.Caching.DependencyInjection;

public class CachingBuilder : ICachingBuilder, IServiceInitializer
{
    private Type? cacheProviderType;

    public ICachingBuilder SetCacheProvider<TCacheProvider>()
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