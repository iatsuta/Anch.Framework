using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSecuritySystem(Action<ISecuritySystemSetup> setupAction) =>
            services.Initialize<SecuritySystemSetup>(setupAction);
    }
}