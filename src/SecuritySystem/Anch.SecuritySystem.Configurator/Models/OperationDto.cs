namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>An operation shown in the configurator.</summary>
public class OperationDto
{
    public required string Name { get; set; }

    public string? Description { get; set; }
}
