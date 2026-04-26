using System.Collections.Immutable;
using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public interface INotificationPermissionExtractor<out TPermission>
{
    IAsyncEnumerable<TPermission> GetPermissionsAsync(ImmutableArray<SecurityRole> securityRoles, ImmutableArray<NotificationFilterGroup> notificationFilterGroups);
}