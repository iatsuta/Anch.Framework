using System.Linq.Expressions;

namespace Anch.SecuritySystem.Notification;

public interface INotificationPermissionFilterFactory<TPermission>
{
    Expression<Func<TPermission, bool>> Create(IEnumerable<SecurityRole> securityRoles);
}
