using System.Collections.Immutable;
using SecuritySystem.Notification.Domain;

namespace SecuritySystem.Notification;

public interface INotificationPermissionExtractor<out TPermission>
{
    IAsyncEnumerable<TPermission> GetPermissionsAsync(ImmutableArray<SecurityRole> securityRoles, ImmutableArray<NotificationFilterGroup> notificationFilterGroups);
}