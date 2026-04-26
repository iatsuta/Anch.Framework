namespace Anch.SecuritySystem.Notification.Domain;

/// <summary>
/// Константы, описывающие правила для вычисления принципалов по роле
/// </summary>
public enum NotificationExpandType
{
    DirectOrEmpty = 0,

    Direct = 1,

    DirectOrFirstParent = 2,

    DirectOrFirstParentOrEmpty = 3,

    All = 4
}