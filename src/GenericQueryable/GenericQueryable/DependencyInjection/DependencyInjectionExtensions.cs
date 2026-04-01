using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace GenericQueryable.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddGenericQueryable(this IServiceCollection services, Action<IGenericQueryableSetup>? setupAction = null) =>
        services.Initialize<GenericQueryableSetup>(setupAction);
}