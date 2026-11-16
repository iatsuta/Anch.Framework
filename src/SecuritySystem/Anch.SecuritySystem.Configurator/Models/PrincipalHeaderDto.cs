namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>A principal without its permissions.</summary>
public record PrincipalHeaderDto : EntityDto
{
    public bool IsVirtual { get; init; }
}
