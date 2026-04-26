using System.Collections.Immutable;

namespace Anch.SecuritySystem.Notification.Domain;

public abstract record NotificationFilterGroup
{
    public abstract Type GetSecurityContextType();

    public required NotificationExpandType ExpandType { get; init; }
}

public record NotificationFilterGroup<TIdent> : NotificationFilterGroup
{
    public required ImmutableArray<TIdent> Idents { get; init; }

    public required Type SecurityContextType { get; init; }

    public override Type GetSecurityContextType() => this.SecurityContextType;
}