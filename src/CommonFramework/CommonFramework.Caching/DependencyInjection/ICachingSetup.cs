namespace CommonFramework.Caching.DependencyInjection;

public interface ICachingSetup
{
    ICachingSetup SetCacheProvider<TCacheProvider>()
        where TCacheProvider : ICacheProvider;
}