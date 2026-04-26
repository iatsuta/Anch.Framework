using Anch.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.IdentitySource.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentitySource(this IServiceCollection services, Action<IIdentitySourceSetup>? setup = null) =>
        services.Initialize<IdentitySourceSetup>(setup);
}