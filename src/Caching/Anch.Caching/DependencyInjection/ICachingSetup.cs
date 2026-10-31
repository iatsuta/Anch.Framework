namespace Anch.Caching.DependencyInjection;

/// <summary>Fluent configuration of the Caching module.</summary>
public interface ICachingSetup
{
    /// <summary>Replaces the default in-memory cache provider.</summary>
    /// <typeparam name="TCacheProvider">The provider type.</typeparam>
    ICachingSetup SetCacheProvider<TCacheProvider>()
        where TCacheProvider : ICacheProvider;
}