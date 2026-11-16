using Anch.GenericRepository;

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Provides queries over the objects of a security context type.</summary>
public class SecurityContextSource(IServiceProvider serviceProvider, IQueryableSource queryableSource) : ISecurityContextSource
{
    public IQueryable<TSecurityContext> GetQueryable<TSecurityContext>(SecurityContextRestrictionFilterInfo<TSecurityContext> filter)
        where TSecurityContext : class, ISecurityContext
    {
        return queryableSource.GetQueryable<TSecurityContext>().Where(filter.GetPureFilter(serviceProvider));
    }
}
