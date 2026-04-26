

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public interface ISecurityContextSource
{
    IQueryable<TSecurityContext> GetQueryable<TSecurityContext>(SecurityContextRestrictionFilterInfo<TSecurityContext> filter)
        where TSecurityContext : class, ISecurityContext;
}
