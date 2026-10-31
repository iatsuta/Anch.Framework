using System.Collections.Immutable;

namespace Anch.SecuritySystem.Notification.Domain;

/// <summary>The security contexts of one type that an event concerns, together with the way permissions are matched against them.</summary>
public abstract record NotificationFilterGroup
{
    /// <summary>Returns the security context type of the group.</summary>
    public abstract Type GetSecurityContextType();

    /// <summary>How permissions are matched against the contexts.</summary>
    public required NotificationExpandType ExpandType { get; init; }
}

/// <summary>A group that holds the identifiers of the security context objects.</summary>
/// <typeparam name="TIdent">The type of the identifier.</typeparam>
public record NotificationFilterGroup<TIdent> : NotificationFilterGroup
{
    /// <summary>The identifiers of the objects.</summary>
    public required ImmutableArray<TIdent> Idents { get; init; }

    /// <summary>The security context type.</summary>
    public required Type SecurityContextType { get; init; }

    /// <inheritdoc/>
    public override Type GetSecurityContextType() => this.SecurityContextType;
}