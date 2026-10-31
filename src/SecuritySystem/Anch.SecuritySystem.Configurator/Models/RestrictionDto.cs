namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>The objects of one security context type that a permission is limited to.</summary>
public class RestrictionDto
{
    public required string Id { get; set; }

    public required string Name { get; set; }
}