using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Caching.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnchCaching(this IServiceCollection services, Action<ICachingSetup>? setup = null) =>
        services.Initialize<CachingSetup>(setup);
}