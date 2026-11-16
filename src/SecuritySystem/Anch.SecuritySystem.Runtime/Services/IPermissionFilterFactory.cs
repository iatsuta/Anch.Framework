using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

/// <summary>Builds the filter that selects the valid permissions (period, delegation) of a permission entity.</summary>
public interface IPermissionFilterFactory<TPermission>
{
    Expression<Func<TPermission, bool>> CreateFilter(SecurityContextRestriction securityContextRestriction);

    Expression<Func<TPermission, bool>> CreateFilter<TSecurityContext>(SecurityContextRestriction<TSecurityContext> securityContextRestriction)
        where TSecurityContext : class, ISecurityContext;
}