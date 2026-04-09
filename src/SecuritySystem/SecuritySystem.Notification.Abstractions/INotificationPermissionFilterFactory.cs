using System.Linq.Expressions;

namespace SecuritySystem.Notification;

public interface INotificationPermissionFilterFactory<TPermission>
{
    Expression<Func<TPermission, bool>> Create(IEnumerable<SecurityRole> securityRoles);
}
