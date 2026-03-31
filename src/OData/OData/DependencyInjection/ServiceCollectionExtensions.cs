using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace OData.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOData(this IServiceCollection services, Action<IODataBuilder>? setupAction = null) =>
        services.Initialize<ODataBuilder>(setupAction);
}