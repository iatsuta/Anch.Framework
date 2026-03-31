namespace CommonFramework.Caching.DependencyInjection;

public interface ICachingBuilder
{
    ICachingBuilder SetCacheProvider<TCacheProvider>()
        where TCacheProvider : ICacheProvider;
}