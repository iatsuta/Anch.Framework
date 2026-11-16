using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.IdentitySource.DependencyInjection;

/// <summary>Registration of the IdentitySource module.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds the identity source, which finds the identifier property of domain types.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setup">Optional configuration.</param>
    public static IServiceCollection AddIdentitySource(this IServiceCollection services, Action<IIdentitySourceSetup>? setup = null) =>
        services.Initialize<IdentitySourceSetup>(setup);
}