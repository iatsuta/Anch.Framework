using System.Collections.Immutable;

namespace Anch.SecuritySystem.Notification.Domain;

/// <summary>A group that holds the security context objects themselves.</summary>
/// <typeparam name="TSecurityContext">The security context type.</typeparam>
public record TypedNotificationFilterGroup<TSecurityContext> : NotificationFilterGroup
    where TSecurityContext : ISecurityContext
{
    /// <summary>The objects.</summary>
    public required ImmutableArray<TSecurityContext> Items { get; init; }

    /// <inheritdoc/>
    public override Type GetSecurityContextType() => typeof(TSecurityContext);
}