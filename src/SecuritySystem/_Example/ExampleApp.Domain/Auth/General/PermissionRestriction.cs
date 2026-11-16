namespace ExampleApp.Domain.Auth.General;

public class PermissionRestriction
{
    public virtual Guid Id { get; init; }

    public virtual Guid SecurityContextId { get; init; }

    public virtual SecurityContextType SecurityContextType { get; init; } = null!;

    public virtual Permission Permission { get; init; } = null!;
}