using System.Collections.Immutable;

using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Configurator.Models;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Anch.SecuritySystem.ExternalSystem.Management;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class GetOperationHandler(
    [WithoutRunAs] ISecuritySystem securitySystem,
    ISecurityRoleSource roleSource,
    IRootPrincipalSourceService principalSourceService)
    : BaseReadHandler, IGetOperationHandler
{
    protected override async Task<object> GetDataAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!await securitySystem.IsSecurityAdministratorAsync(cancellationToken))
        {
            return new OperationDetailsDto { BusinessRoles = [], Principals = [] };
        }
        else
        {
            var securityOperation = new SecurityOperation(context.ExtractName());

            var securityRoles = roleSource.SecurityRoles
                .Where(x => x.Information.Operations.Contains(securityOperation))
                .ToImmutableHashSet<SecurityRole>();

            var principals = await principalSourceService.GetLinkedPrincipalsAsync(securityRoles).ToArrayAsync(cancellationToken);

            return new OperationDetailsDto
            {
                BusinessRoles = securityRoles.Select(x => x.Name).Order().ToArray(),
                Principals = principals
            };
        }
    }
}