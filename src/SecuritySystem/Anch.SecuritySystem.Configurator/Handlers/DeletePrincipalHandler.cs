using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Anch.SecuritySystem.ExternalSystem.Management;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class DeletePrincipalHandler(
    [WithoutRunAs] ISecuritySystem securitySystem,
    IPrincipalManagementService principalManagementService,
    IConfiguratorIntegrationEvents? configuratorIntegrationEvents = null)
    : BaseWriteHandler, IDeletePrincipalHandler
{
    public async Task Execute(HttpContext context, CancellationToken ct)
    {
        await securitySystem.CheckAccessAsync(ApplicationSecurityRule.SecurityAdministrator, ct);

        var principal = await principalManagementService.RemovePrincipalAsync(context.ExtractSecurityIdentity(), false, ct);

        if (configuratorIntegrationEvents != null)
            await configuratorIntegrationEvents.PrincipalRemovedAsync(principal, ct);
    }
}