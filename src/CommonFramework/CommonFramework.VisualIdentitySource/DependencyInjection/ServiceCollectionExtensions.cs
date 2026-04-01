using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.VisualIdentitySource.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVisualIdentitySource(this IServiceCollection services, Action<IVisualIdentitySourceSetup>? setupAction = null) =>
        services.Initialize<VisualIdentitySourceSetup>(setupAction);
}