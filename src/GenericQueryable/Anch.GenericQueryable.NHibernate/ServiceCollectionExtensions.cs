using Anch.Core;
using Anch.GenericQueryable.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.NHibernate;

/// <summary>Registration of GenericQueryable for NHibernate.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds GenericQueryable with the NHibernate fetch service and method extractor. Also call <c>SetGenericQueryProvider</c> on the NHibernate configuration.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setupAction">Optional configuration, for example custom fetch rule expanders.</param>
    public static IServiceCollection AddNHibernateGenericQueryable(this IServiceCollection services, Action<IGenericQueryableSetup>? setupAction = null)
    {
        return services.AddGenericQueryable(v => v
            .SetFetchService<NHibFetchService>()
            .SetTargetMethodExtractor<NHibTargetMethodExtractor>()
            .Pipe(builder => setupAction?.Invoke(builder)));
    }
}