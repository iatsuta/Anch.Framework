using Anch.SecuritySystem;

namespace ExampleApp.Domain;

public class Location : ISecurityContext
{
    public virtual int MyId { get; set; }

    public virtual required string Name { get; set; }
}