using Anch.SecuritySystem;

namespace ExampleApp.Domain;

public class Employee : ISecurityContext
{
    public virtual Guid Id { get; set; }

    public virtual required string Login { get; set; }
}