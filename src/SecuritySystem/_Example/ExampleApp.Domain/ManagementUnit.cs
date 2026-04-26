using Anch.SecuritySystem;

namespace ExampleApp.Domain;

public class ManagementUnit : ISecurityContext
{
    public virtual Guid Id { get; set; }

    public virtual int DeepLevel { get; set; }

    public virtual ManagementUnit? Parent { get; set; }

    public virtual required string Name { get; set; }
}