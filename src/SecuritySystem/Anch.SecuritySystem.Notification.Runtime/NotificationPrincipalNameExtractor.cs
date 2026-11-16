using System.Collections.Immutable;

using Anch.Core;
using Anch.SecuritySystem.Notification.Domain;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.Notification;

/// <summary>Finds the names of the principals to notify for an event.</summary>
public class NotificationPrincipalNameExtractor(IServiceProxyFactory serviceProxyFactory, IEnumerable<PermissionBindingInfo> bindingInfoList)
    : INotificationPrincipalNameExtractor
{
    private readonly Lazy<INotificationPrincipalNameExtractor[]> lazyInnerServices = new(() =>
        bindingInfoList.Select(permissionBindingInfo => permissionBindingInfo.PrincipalType).Distinct()
                .Select(principalType => serviceProxyFactory.Create<INotificationPrincipalNameExtractor>(typeof(NotificationPrincipalNameExtractor<>).MakeGenericType(principalType))).ToArray());

    public IAsyncEnumerable<string> GetPrincipalNamesAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule,
        ImmutableArray<NotificationFilterGroup> notificationFilterGroups) =>
        this.lazyInnerServices.Value.ToAsyncEnumerable()
            .SelectMany(innerService => innerService.GetPrincipalNamesAsync(securityRule, notificationFilterGroups))
            .Distinct();
}

/// <summary>Finds the names of the principals to notify for an event.</summary>
public class NotificationPrincipalNameExtractor<TPrincipal>(
    INotificationPrincipalExtractor<TPrincipal> notificationPrincipalExtractor,
    IVisualIdentityInfo<TPrincipal> visualIdentityInfo) : INotificationPrincipalNameExtractor
{
    public IAsyncEnumerable<string> GetPrincipalNamesAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule,
        ImmutableArray<NotificationFilterGroup> notificationFilterGroups) => notificationPrincipalExtractor
        .GetPrincipalsAsync(securityRule, notificationFilterGroups).Select(visualIdentityInfo.Name.Getter);
}