using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Configurator.Models;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Anch.SecuritySystem.ExternalSystem.Management;
using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class GetBusinessRoleHandler(
    [WithoutRunAs] ISecuritySystem securitySystem,
    ISecurityRoleSource securityRoleSource,
    ISecurityOperationInfoSource securityOperationInfoSource,
    IRootPrincipalSourceService principalSourceService)
    : BaseReadHandler, IGetBusinessRoleHandler
{
    protected override async Task<object> GetDataAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!await securitySystem.IsSecurityAdministratorAsync(cancellationToken))
        {
            return new BusinessRoleDetailsDto { Operations = [], Principals = [] };
        }
        else
        {
            var securityRole = securityRoleSource.GetSecurityRole(context.ExtractSecurityIdentity());

            var operations =
                securityRole
                    .Information
                    .Operations
                    .Select(o => new OperationDto
                    {
                        Name = o.Name,
                        Description = securityOperationInfoSource.GetSecurityOperationInfo(o).Description
                    })
                    .OrderBy(x => x.Name)
                    .ToArray();

            var principals = await principalSourceService.GetLinkedPrincipalsAsync([securityRole]).ToArrayAsync(cancellationToken);

            return new BusinessRoleDetailsDto { Operations = operations, Principals = principals };
        }
    }
}