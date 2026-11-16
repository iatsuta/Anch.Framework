using System.Linq.Expressions;

using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Notification;

/// <summary>Builds the filter that selects the permissions that are valid for notifications.</summary>
public class NotificationGeneralPermissionFilterFactory<TPermission>(IAvailablePermissionFilterFactory<TPermission> availablePermissionFilterFactory)
    : INotificationPermissionFilterFactory<TPermission>
{
    public Expression<Func<TPermission, bool>> Create(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
        availablePermissionFilterFactory.CreateFilter(securityRule.TryApply(new SecurityRuleCredential.AnyUserCredential()));
}