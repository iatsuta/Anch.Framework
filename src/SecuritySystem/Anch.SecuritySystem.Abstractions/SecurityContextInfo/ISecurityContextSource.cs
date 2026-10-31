

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Provides queries over the objects of a security context type.</summary>
public interface ISecurityContextSource
{
    /// <summary>Returns the objects that pass the filter.</summary>
    /// <param name="filter">Limits the returned objects.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    IQueryable<TSecurityContext> GetQueryable<TSecurityContext>(SecurityContextRestrictionFilterInfo<TSecurityContext> filter)
        where TSecurityContext : class, ISecurityContext;
}
