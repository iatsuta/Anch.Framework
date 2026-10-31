namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>Details of an operation: its description and the roles that include it.</summary>
public class OperationDetailsDto
{
    public required string[] BusinessRoles { get; set; }

    public required string[] Principals { get; set; }
}
