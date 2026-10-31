using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DependencyInjection;

/// <summary>Registration of the SecuritySystem module.</summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Adds the security system to the container; everything is configured in <paramref name="setupAction"/>.</summary>
        /// <param name="setupAction">Configures roles, contexts, domain object security, data access and the current user.</param>
        public IServiceCollection AddSecuritySystem(Action<ISecuritySystemSetup> setupAction) =>
            services.Initialize<SecuritySystemSetup>(setupAction);
    }
}