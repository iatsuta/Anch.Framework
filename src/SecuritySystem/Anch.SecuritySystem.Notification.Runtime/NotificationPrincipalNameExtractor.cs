using System.Collections.Immutable;

using Anch.Core;
using Anch.SecuritySystem.Notification.Domain;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.Notification;

public class NotificationPrincipalNameExtractor(IServiceProxyFactory serviceProxyFactory, IEnumerable<PermissionBindingInfo> bindingInfoList)
    : INotificationPrincipalNameExtractor
{
    private readonly Lazy<INotificationPrincipalNameExtractor[]> lazyInnerServices = new(() =>
        bindingInfoList.Select(permissionBindingInfo => permissionBindingInfo.PrincipalType).Distinct()
                .Select(principalType => serviceProxyFactory.Create<INotificationPrincipalNameExtractor>(typeof(NotificationPrincipalNameExtractor<>).MakeGenericType(principalType))).ToArray());

    public IAsyncEnumerable<string> GetPrincipalNamesAsync(ImmutableArray<SecurityRole> securityRoles,
        ImmutableArray<NotificationFilterGroup> notificationFilterGroups) =>
        this.lazyInnerServices.Value.ToAsyncEnumerable()
            .SelectMany(innerService => innerService.GetPrincipalNamesAsync(securityRoles, notificationFilterGroups))
            .Distinct();
}

public class NotificationPrincipalNameExtractor<TPrincipal>(
    INotificationPrincipalExtractor<TPrincipal> notificationPrincipalExtractor,
    IVisualIdentityInfo<TPrincipal> visualIdentityInfo) : INotificationPrincipalNameExtractor
{
    public IAsyncEnumerable<string> GetPrincipalNamesAsync(ImmutableArray<SecurityRole> securityRoles,
        ImmutableArray<NotificationFilterGroup> notificationFilterGroups) => notificationPrincipalExtractor
        .GetPrincipalsAsync(securityRoles, notificationFilterGroups).Select(visualIdentityInfo.Name.Getter);
}