using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton(typeof(ICacheProvider), this.cacheProviderType ?? typeof(CacheProvider));
        }
    }
}