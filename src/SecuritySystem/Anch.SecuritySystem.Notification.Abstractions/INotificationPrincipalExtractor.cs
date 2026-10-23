using System.Collections.Immutable;

using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public interface INotificationPrincipalExtractor<out TPrincipal>
{
    IAsyncEnumerable<TPrincipal> GetPrincipalsAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, ImmutableArray<NotificationFilterGroup> notificationFilterGroups);
}