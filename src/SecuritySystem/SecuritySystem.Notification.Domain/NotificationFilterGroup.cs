using System.Collections.Immutable;

namespace SecuritySystem.Notification.Domain;

public abstract record NotificationFilterGroup
{
    public required Type SecurityContextType { get; init; }

    public required NotificationExpandType ExpandType { get; init; }
};

public record NotificationFilterGroup<TIdent> : NotificationFilterGroup
{
    public required ImmutableArray<TIdent> Idents { get; init; }
}