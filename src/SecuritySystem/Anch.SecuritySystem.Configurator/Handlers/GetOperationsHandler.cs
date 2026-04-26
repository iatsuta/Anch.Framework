using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Configurator.Models;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class GetOperationsHandler([WithoutRunAs]ISecuritySystem securitySystem, ISecurityRoleSource roleSource, ISecurityOperationInfoSource operationInfoSource)
    : BaseReadHandler, IGetOperationsHandler
{
    protected override async Task<object> GetDataAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!await securitySystem.IsSecurityAdministratorAsync(cancellationToken)) return new List<string>();

        var operations = roleSource.SecurityRoles
                                   .SelectMany(x => x.Information.Operations)
                                   .Select(
                                       o => new OperationDto
                                            {
                                                Name = o.Name, Description = operationInfoSource.GetSecurityOperationInfo(o).Description
                                            })
                                   .OrderBy(x => x.Name)
                                   .DistinctBy(x => x.Name)
                                   .ToList();

        return operations;
    }
}
