using System.Linq.Expressions;

using SecuritySystem.Services;

namespace SecuritySystem.Notification;

public class NotificationGeneralPermissionFilterFactory<TPermission>(IAvailablePermissionFilterFactory<TPermission> availablePermissionFilterFactory)
    : INotificationPermissionFilterFactory<TPermission>
{
    public Expression<Func<TPermission, bool>> Create(IEnumerable<SecurityRole> securityRoles) =>
        availablePermissionFilterFactory.CreateFilter(
            new DomainSecurityRule.ExpandedRolesSecurityRule(securityRoles) { CustomCredential = new SecurityRuleCredential.AnyUserCredential() });
}