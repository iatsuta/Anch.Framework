namespace Anch.SecuritySystem.Notification.Domain;

/// <summary>Extensions for <see cref="NotificationExpandType"/>.</summary>
public static class NotificationExpandTypeExtensions
{
    extension(NotificationExpandType notificationExpandType)
    {
        /// <summary>Determines whether the mode looks at ancestors of the context.</summary>
        public bool IsHierarchical() =>
            notificationExpandType switch
            {
                NotificationExpandType.DirectOrFirstParent or NotificationExpandType.DirectOrFirstParentOrEmpty or NotificationExpandType.All => true,
                _ => false
            };

        /// <summary>Determines whether finding no matching permission is acceptable.</summary>
        public bool AllowEmpty() =>
            notificationExpandType switch
            {
                NotificationExpandType.DirectOrEmpty or NotificationExpandType.DirectOrFirstParentOrEmpty or NotificationExpandType.All => true,
                _ => false
            };
    }
}
