namespace Anch.SecuritySystem.Configurator.Models;

public class OperationDetailsDto
{
    public required string[] BusinessRoles { get; set; }

    public required string[] Principals { get; set; }
}
