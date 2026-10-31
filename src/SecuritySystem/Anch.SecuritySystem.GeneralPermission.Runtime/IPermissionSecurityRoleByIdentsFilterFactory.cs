using System.Linq.Expressions;

namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Builds the filter that selects the permissions of given role identifiers.</summary>
public interface IPermissionSecurityRoleByIdentsFilterFactory<TPermission>
{
    Expression<Func<TPermission, bool>> CreateFilter(Type identType, Array idents);
}