using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.VisualIdentitySource.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVisualIdentitySource(this IServiceCollection services, Action<IVisualIdentitySourceSetup>? setupAction = null) =>
        services.Initialize<VisualIdentitySourceSetup>(setupAction);
}