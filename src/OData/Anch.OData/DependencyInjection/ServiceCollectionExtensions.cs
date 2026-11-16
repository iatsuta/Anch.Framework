using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.OData.DependencyInjection;

/// <summary>Registration of the OData module.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds the OData parsers (and the caching they use).</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setupAction">Optional configuration.</param>
    public static IServiceCollection AddOData(this IServiceCollection services, Action<IODataSetup>? setupAction = null) =>
        services.Initialize<ODataSetup>(setupAction);
}