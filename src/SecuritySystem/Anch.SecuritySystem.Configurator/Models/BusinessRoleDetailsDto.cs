namespace Anch.SecuritySystem.Configurator.Models;

public class BusinessRoleDetailsDto
{
    public required OperationDto[] Operations { get; set; }

    public required string[] Principals { get; set; }
}
