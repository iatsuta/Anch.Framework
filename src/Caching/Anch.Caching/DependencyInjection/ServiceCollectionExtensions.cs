using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Caching.DependencyInjection;

/// <summary>Registration of the Caching module.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds the cache provider and caches.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setup">Optional configuration, for example a custom cache provider.</param>
    public static IServiceCollection AddAnchCaching(this IServiceCollection services, Action<ICachingSetup>? setup = null) =>
        services.Initialize<CachingSetup>(setup);
}