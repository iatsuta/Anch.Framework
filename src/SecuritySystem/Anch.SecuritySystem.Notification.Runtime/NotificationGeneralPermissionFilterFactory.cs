using System.Linq.Expressions;

using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Notification;

public class NotificationGeneralPermissionFilterFactory<TPermission>(IAvailablePermissionFilterFactory<TPermission> availablePermissionFilterFactory)
    : INotificationPermissionFilterFactory<TPermission>
{
    public Expression<Func<TPermission, bool>> Create(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
        availablePermissionFilterFactory.CreateFilter(securityRule.TryApply(new SecurityRuleCredential.AnyUserCredential()));
}