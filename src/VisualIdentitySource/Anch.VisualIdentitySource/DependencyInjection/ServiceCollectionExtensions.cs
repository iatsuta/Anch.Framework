using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.VisualIdentitySource.DependencyInjection;

/// <summary>Registration of the VisualIdentitySource module.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds the visual identity source, which finds the display name of domain types.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setupAction">Optional configuration.</param>
    public static IServiceCollection AddVisualIdentitySource(this IServiceCollection services, Action<IVisualIdentitySourceSetup>? setupAction = null) =>
        services.Initialize<VisualIdentitySourceSetup>(setupAction);
}