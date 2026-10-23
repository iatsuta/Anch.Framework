using System.Collections.Immutable;

using Anch.Core;
using Anch.SecuritySystem.Notification.Domain;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Notification;

public class NotificationPrincipalExtractor<TPrincipal>(IServiceProxyFactory serviceProxyFactory, IPermissionBindingInfoSource permissionBindingInfoSource)
    : INotificationPrincipalExtractor<TPrincipal>
{
    private readonly Lazy<INotificationPrincipalExtractor<TPrincipal>[]> lazyInnerServices = new(() =>
        permissionBindingInfoSource.GetForPrincipal(typeof(TPrincipal)).Select(permissionBindingInfo =>
                serviceProxyFactory.Create<INotificationPrincipalExtractor<TPrincipal>>(
                    typeof(NotificationPrincipalExtractor<,>).MakeGenericType(permissionBindingInfo.PrincipalType, permissionBindingInfo.PermissionType)))
            .ToArray());

    public IAsyncEnumerable<TPrincipal> GetPrincipalsAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule,
        ImmutableArray<NotificationFilterGroup> notificationFilterGroups) =>
        this.lazyInnerServices.Value switch
        {
            [] => throw new InvalidOperationException($"NotificationPrincipalExtractors for {nameof(TPrincipal)} not exists"),

            [var innerService] => innerService.GetPrincipalsAsync(securityRule, notificationFilterGroups),

            _ => this.lazyInnerServices.Value.ToAsyncEnumerable()
                .SelectMany(innerService => innerService.GetPrincipalsAsync(securityRule, notificationFilterGroups))
        };
}

public class NotificationPrincipalExtractor<TPrincipal, TPermission>(
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo,
    INotificationPermissionExtractor<TPermission> notificationPermissionExtractor)
    : INotificationPrincipalExtractor<TPrincipal>
{
    public IAsyncEnumerable<TPrincipal> GetPrincipalsAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule,
        ImmutableArray<NotificationFilterGroup> notificationFilterGroups) =>

        notificationPermissionExtractor
            .GetPermissionsAsync(securityRule, notificationFilterGroups)
            .Select(bindingInfo.Principal.Getter)
            .Distinct();
}