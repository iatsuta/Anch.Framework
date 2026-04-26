using System.Collections.Immutable;

using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public interface INotificationPrincipalExtractor<out TPrincipal>
{
    IAsyncEnumerable<TPrincipal> GetPrincipalsAsync(ImmutableArray<SecurityRole> securityRoles, ImmutableArray<NotificationFilterGroup> notificationFilterGroups);
}