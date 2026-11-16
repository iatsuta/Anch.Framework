namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>A role with its identifier and name.</summary>
public record FullRoleDto : EntityDto
{
    public bool IsVirtual { get; init; }

    public required IReadOnlyList<RoleContextDto> Contexts { get; init; }
}
