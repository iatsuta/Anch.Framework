using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.IdentitySource.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentitySource(this IServiceCollection services, Action<IIdentitySourceSetup>? setup = null) =>
        services.Initialize<IdentitySourceSetup>(setup);
}