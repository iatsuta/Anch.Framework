using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Testing.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSecuritySystemTesting(Action<ISecuritySystemTestingSetup>? setup = null) =>
            services.Initialize<SecuritySystemTestingSetup>(setup);
    }
}