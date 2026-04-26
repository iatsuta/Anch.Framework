using Anch.Core;
using Anch.GenericQueryable.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.NHibernate;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNHibernateGenericQueryable(this IServiceCollection services, Action<IGenericQueryableSetup>? setupAction = null)
    {
        return services.AddGenericQueryable(v => v
            .SetFetchService<NHibFetchService>()
            .SetTargetMethodExtractor<NHibTargetMethodExtractor>()
            .Pipe(builder => setupAction?.Invoke(builder)));
    }
}