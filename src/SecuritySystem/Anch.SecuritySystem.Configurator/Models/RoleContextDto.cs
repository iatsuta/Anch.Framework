namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>A security context type that a role is limited by.</summary>
public record RoleContextDto(string Name, bool Required);
