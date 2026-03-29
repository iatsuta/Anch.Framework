using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace SecuritySystem.Testing.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSecuritySystemTesting(Action<ISecuritySystemTestingBuilder>? setup = null) =>
            services.Initialize<SecuritySystemTestingBuilder>(setup);
    }
}