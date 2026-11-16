namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>A principal with its permissions.</summary>
public class PrincipalDetailsDto
{
    public required List<PermissionDto> Permissions { get; set; }
}
