using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.OData.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOData(this IServiceCollection services, Action<IODataSetup>? setupAction = null) =>
        services.Initialize<ODataSetup>(setupAction);
}