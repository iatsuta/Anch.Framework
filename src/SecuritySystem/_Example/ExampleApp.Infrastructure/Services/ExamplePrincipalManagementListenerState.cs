namespace ExampleApp.Infrastructure.Services;

public record ExamplePrincipalManagementListenerState
{
    public List<Guid> CreatedPrincipals { get; } = new List<Guid>();
}