using System.Collections.Immutable;

namespace Anch.SecuritySystem.Notification.Domain;

public record TypedNotificationFilterGroup<TSecurityContext> : NotificationFilterGroup
    where TSecurityContext : ISecurityContext
{
    public required ImmutableArray<TSecurityContext> Items { get; init; }

    public override Type GetSecurityContextType() => typeof(TSecurityContext);
}