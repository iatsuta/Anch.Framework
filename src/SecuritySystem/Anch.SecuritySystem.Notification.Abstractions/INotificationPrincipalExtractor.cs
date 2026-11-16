using System.Collections.Immutable;

using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

/// <summary>Finds the principals that must be notified about an event that concerns given security contexts.</summary>
/// <typeparam name="TPrincipal">The principal entity type.</typeparam>
public interface INotificationPrincipalExtractor<out TPrincipal>
{
    /// <summary>Returns the principals to notify.</summary>
    /// <param name="securityRule">The rule that the principals' permissions must satisfy (usually a role).</param>
    /// <param name="notificationFilterGroups">The security contexts of the event, one group per context type, with the way they are expanded.</param>
    IAsyncEnumerable<TPrincipal> GetPrincipalsAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, ImmutableArray<NotificationFilterGroup> notificationFilterGroups);
}