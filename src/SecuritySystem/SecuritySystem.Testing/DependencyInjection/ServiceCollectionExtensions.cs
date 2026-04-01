using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace SecuritySystem.Testing.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSecuritySystemTesting(Action<ISecuritySystemTestingSetup>? setup = null) =>
            services.Initialize<SecuritySystemTestingSetup>(setup);
    }
}