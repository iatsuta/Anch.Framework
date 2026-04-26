using Anch.SecuritySystem;

namespace ExampleApp.Domain;

public class BusinessUnit : ISecurityContext
{
    public virtual Guid Id { get; set; }

    public virtual int DeepLevel { get; set; }

    public virtual BusinessUnit? Parent { get; set; }

    public virtual required string Name { get; set; }

    public virtual bool AllowedForFilterRole { get; set; }
}