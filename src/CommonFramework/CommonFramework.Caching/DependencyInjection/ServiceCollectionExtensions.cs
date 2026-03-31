using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Caching.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, Action<ICachingBuilder>? setup = null) =>
        services.Initialize<CachingBuilder>(setup);
}