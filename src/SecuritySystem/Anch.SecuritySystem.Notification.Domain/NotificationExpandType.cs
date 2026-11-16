namespace Anch.SecuritySystem.Notification.Domain;

/// <summary>
/// Constants describing the rules for resolving principals by role.
/// </summary>
public enum NotificationExpandType
{
    /// <summary>Permissions that name the context directly; none is acceptable.</summary>
    DirectOrEmpty = 0,

    /// <summary>Permissions that name the context directly; at least one is expected.</summary>
    Direct = 1,

    /// <summary>Permissions that name the context directly, or, if there are none, those of the nearest ancestor that has any.</summary>
    DirectOrFirstParent = 2,

    /// <summary>Like <see cref="DirectOrFirstParent"/>, but no match at all is acceptable.</summary>
    DirectOrFirstParentOrEmpty = 3,

    /// <summary>Permissions that name the context or any of its ancestors; none is acceptable.</summary>
    All = 4
}