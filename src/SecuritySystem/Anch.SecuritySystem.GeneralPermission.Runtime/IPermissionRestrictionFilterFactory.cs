using System.Linq.Expressions;

namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Builds the filter that selects the restrictions that allow given security context objects.</summary>
public interface IPermissionRestrictionFilterFactory<TPermissionRestriction>
{
    Expression<Func<TPermissionRestriction, bool>> CreateFilter<TSecurityContext>(SecurityContextRestrictionFilterInfo<TSecurityContext> restrictionFilterInfo)
        where TSecurityContext : class, ISecurityContext;
}