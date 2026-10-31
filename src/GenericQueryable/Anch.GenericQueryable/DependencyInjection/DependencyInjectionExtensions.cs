using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.DependencyInjection;

/// <summary>Registration of the GenericQueryable module.</summary>
public static class DependencyInjectionExtensions
{
    /// <summary>Adds the services behind the ORM-independent LINQ operators and <c>WithFetch</c>. ORM adapters call it with their own fetch service and method extractor.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setupAction">Optional configuration.</param>
    public static IServiceCollection AddGenericQueryable(this IServiceCollection services, Action<IGenericQueryableSetup>? setupAction = null) =>
        services.Initialize<GenericQueryableSetup>(setupAction);
}