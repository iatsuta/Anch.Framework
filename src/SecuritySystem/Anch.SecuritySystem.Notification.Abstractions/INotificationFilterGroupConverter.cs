using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

/// <summary>Converts a notification filter group that holds security context objects into one that holds their identifiers.</summary>
public interface INotificationFilterGroupConverter
{
    /// <summary>Converts the group.</summary>
    /// <param name="notificationFilterGroup">The group to convert.</param>
    /// <typeparam name="TSecurityContextIdent">The type of the security context identifier.</typeparam>
    NotificationFilterGroup<TSecurityContextIdent> Convert<TSecurityContextIdent>(NotificationFilterGroup notificationFilterGroup)
        where TSecurityContextIdent : notnull;
}