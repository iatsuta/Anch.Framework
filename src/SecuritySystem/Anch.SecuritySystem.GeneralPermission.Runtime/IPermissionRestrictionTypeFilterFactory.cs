using System.Linq.Expressions;

namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Builds the filter that selects the restrictions of a security context type.</summary>
public interface IPermissionRestrictionTypeFilterFactory<TPermissionRestriction>
{
    Expression<Func<TPermissionRestriction, bool>> CreateFilter<TSecurityContext>()
        where TSecurityContext : class, ISecurityContext;
}