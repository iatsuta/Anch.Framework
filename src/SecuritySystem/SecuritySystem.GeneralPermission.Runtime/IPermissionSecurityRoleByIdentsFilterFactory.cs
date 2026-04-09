using System.Linq.Expressions;

namespace SecuritySystem.GeneralPermission;

public interface IPermissionSecurityRoleByIdentsFilterFactory<TPermission>
{
    Expression<Func<TPermission, bool>> CreateFilter(Type identType, Array idents);
}