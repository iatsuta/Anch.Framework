using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Testing.DependencyInjection;

/// <summary>Registration of the testing helpers.</summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Adds helpers that run test code on behalf of a test user with given permissions.</summary>
        /// <param name="setup">Optional configuration.</param>
        public IServiceCollection AddSecuritySystemTesting(Action<ISecuritySystemTestingSetup>? setup = null) =>
            services.Initialize<SecuritySystemTestingSetup>(setup);
    }
}