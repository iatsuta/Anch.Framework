using System.Collections.Concurrent;
using Anch.Core;
using Anch.IdentitySource;
using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public class NotificationFilterGroupConverter(IIdentityInfoSource identityInfoSource) : INotificationFilterGroupConverter
{
    private readonly ConcurrentDictionary<Type, Delegate> cache = [];

    public NotificationFilterGroup<TSecurityContextIdent> Convert<TSecurityContextIdent>(NotificationFilterGroup notificationFilterGroup)
        where TSecurityContextIdent : notnull
    {
        if (notificationFilterGroup is NotificationFilterGroup<TSecurityContextIdent> rawNotificationFilterGroup)
        {
            return rawNotificationFilterGroup;
        }
        else
        {
            return this.cache.GetOrAddAs(notificationFilterGroup.GetSecurityContextType(), securityContextType =>
                new Func<Func<NotificationFilterGroup, NotificationFilterGroup<TSecurityContextIdent>>>(
                        this.GetConvertFunc<ISecurityContext, TSecurityContextIdent>)
                    .CreateGenericMethod(securityContextType, typeof(TSecurityContextIdent))
                    .Invoke<Func<NotificationFilterGroup, NotificationFilterGroup<TSecurityContextIdent>>>(this)).Invoke(notificationFilterGroup);
        }
    }

    private Func<NotificationFilterGroup, NotificationFilterGroup<TSecurityContextIdent>> GetConvertFunc<TSecurityContext, TSecurityContextIdent>()
        where TSecurityContext : ISecurityContext
        where TSecurityContextIdent : notnull
    {
        var identityInfo = identityInfoSource.GetIdentityInfo<TSecurityContext, TSecurityContextIdent>();

        return notificationFilterGroup =>
            new NotificationFilterGroup<TSecurityContextIdent>
            {
                ExpandType = notificationFilterGroup.ExpandType,
                SecurityContextType = typeof(TSecurityContext),
                Idents = [.. ((TypedNotificationFilterGroup<TSecurityContext>)notificationFilterGroup).Items.Select(identityInfo.Id.Getter)]
            };
    }
}