namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>An object with an identifier and a name.</summary>
public record EntityDto
{
    public required string Id { get; init; }

    public required string Name { get; init; }
}