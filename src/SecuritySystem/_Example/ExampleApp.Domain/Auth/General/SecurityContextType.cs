namespace ExampleApp.Domain.Auth.General;

public class SecurityContextType
{
    public virtual Guid Id { get; init; }

    public virtual string Name { get; set; } = null!;
}