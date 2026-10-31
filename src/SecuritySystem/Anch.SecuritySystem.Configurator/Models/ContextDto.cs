namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>A security context object shown in the configurator.</summary>
public record ContextDto : EntityDto
{
    public required List<RestrictionDto> Entities { get; init; }
}
