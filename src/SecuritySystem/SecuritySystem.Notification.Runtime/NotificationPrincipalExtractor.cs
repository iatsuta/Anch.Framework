using CommonFramework;

using SecuritySystem.Services;

using System.Collections.Immutable;
using SecuritySystem.Notification.Domain;

namespace SecuritySystem.Notification;

public class NotificationPrincipalExtractor<TPrincipal>(IServiceProxyFactory serviceProxyFactory, IPermissionBindingInfoSource permissionBindingInfoSource)
    : INotificationPrincipalExtractor<TPrincipal>
{
    private readonly Lazy<INotificationPrincipalExtractor<TPrincipal>[]> lazyInnerServices = new(() =>
        permissionBindingInfoSource.GetForPrincipal(typeof(TPrincipal)).Select(permissionBindingInfo =>
                serviceProxyFactory.Create<INotificationPrincipalExtractor<TPrincipal>>(
                    typeof(NotificationPrincipalExtractor<,>).MakeGenericType(permissionBindingInfo.PrincipalType, permissionBindingInfo.PermissionType)))
            .ToArray());

    public IAsyncEnumerable<TPrincipal> GetPrincipalsAsync(ImmutableArray<SecurityRole> securityRoles,
        ImmutableArray<NotificationFilterGroup> notificationFilterGroups) =>

        lazyInnerServices.Value switch
        {
            [] => throw new InvalidOperationException($"NotificationPrincipalExtractors for {nameof(TPrincipal)} not exists"),

            [var innerService] => innerService.GetPrincipalsAsync(securityRoles, notificationFilterGroups),

            _ => lazyInnerServices.Value.ToAsyncEnumerable()
                .SelectMany(innerService => innerService.GetPrincipalsAsync(securityRoles, notificationFilterGroups))
        };
}

public class NotificationPrincipalExtractor<TPrincipal, TPermission>(
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo,
    INotificationPermissionExtractor<TPermission> notificationPermissionExtractor)
    : INotificationPrincipalExtractor<TPrincipal>
{
    public IAsyncEnumerable<TPrincipal> GetPrincipalsAsync(ImmutableArray<SecurityRole> securityRoles, ImmutableArray<NotificationFilterGroup> notificationFilterGroups) =>

        notificationPermissionExtractor
            .GetPermissionsAsync(securityRoles, notificationFilterGroups)
            .Select(bindingInfo.Principal.Getter)
            .Distinct();
}