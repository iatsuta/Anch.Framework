using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddGenericQueryable(this IServiceCollection services, Action<IGenericQueryableSetup>? setupAction = null) =>
        services.Initialize<GenericQueryableSetup>(setupAction);
}