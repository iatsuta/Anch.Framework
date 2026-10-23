using System.Collections.Immutable;

using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public interface INotificationPrincipalNameExtractor
{
    IAsyncEnumerable<string> GetPrincipalNamesAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, ImmutableArray<NotificationFilterGroup> notificationFilterGroups);
}