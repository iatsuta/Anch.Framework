using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Caching.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonCaching(this IServiceCollection services, Action<ICachingSetup>? setup = null) =>
        services.Initialize<CachingSetup>(setup);
}