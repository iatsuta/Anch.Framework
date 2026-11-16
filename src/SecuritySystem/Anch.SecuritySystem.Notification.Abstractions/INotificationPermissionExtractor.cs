using System.Collections.Immutable;

using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

/// <summary>Finds the permissions whose holders must be notified about an event that concerns given security contexts.</summary>
/// <typeparam name="TPermission">The permission entity type.</typeparam>
public interface INotificationPermissionExtractor<out TPermission>
{
    /// <summary>Returns the matching permissions.</summary>
    /// <param name="securityRule">The rule that the permissions must satisfy (usually a role).</param>
    /// <param name="notificationFilterGroups">The security contexts of the event, one group per context type, with the way they are expanded.</param>
    IAsyncEnumerable<TPermission> GetPermissionsAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, ImmutableArray<NotificationFilterGroup> notificationFilterGroups);
}