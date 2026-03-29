using System.Linq.Expressions;
using SecuritySystem.Notification.Domain;

namespace SecuritySystem.Notification;

public interface IPermissionLevelInfoExtractor<TPermission>
{
    Expression<Func<PermissionLevelInfo<TPermission>, FullPermissionLevelInfo<TPermission>>> GetSelector(NotificationFilterGroup notificationFilterGroup);
}
