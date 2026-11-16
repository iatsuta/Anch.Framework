namespace Anch.SecuritySystem.Configurator.Models;

/// <summary>Details of a role: its description, operations, contexts and the principals that have it.</summary>
public class BusinessRoleDetailsDto
{
    public required OperationDto[] Operations { get; set; }

    public required string[] Principals { get; set; }
}
