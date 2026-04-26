using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public interface INotificationFilterGroupConverter
{
    NotificationFilterGroup<TSecurityContextIdent> Convert<TSecurityContextIdent>(NotificationFilterGroup notificationFilterGroup)
        where TSecurityContextIdent : notnull;
}