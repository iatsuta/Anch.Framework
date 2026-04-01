using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace SecuritySystem.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSecuritySystem(Action<ISecuritySystemSetup> setupAction) =>
            services.Initialize<SecuritySystemSetup>(setupAction);
    }
}