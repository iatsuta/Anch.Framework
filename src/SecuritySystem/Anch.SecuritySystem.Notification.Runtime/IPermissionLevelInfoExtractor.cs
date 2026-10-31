using System.Linq.Expressions;

using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

/// <summary>Extracts how deep in the hierarchy the restrictions of a permission are.</summary>
public interface IPermissionLevelInfoExtractor<TPermission>
{
    Expression<Func<PermissionLevelInfo<TPermission>, FullPermissionLevelInfo<TPermission>>> GetSelector(NotificationFilterGroup notificationFilterGroup);
}
