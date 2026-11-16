using System.Linq.Expressions;

namespace Anch.SecuritySystem.Notification;

/// <summary>Builds the filter that selects the permissions relevant for notifications.</summary>
/// <typeparam name="TPermission">The permission entity type.</typeparam>
public interface INotificationPermissionFilterFactory<TPermission>
{
    /// <summary>Builds the filter.</summary>
    /// <param name="securityRule">The rule that the permissions must satisfy.</param>
    Expression<Func<TPermission, bool>> Create(DomainSecurityRule.RoleBaseSecurityRule securityRule);
}
