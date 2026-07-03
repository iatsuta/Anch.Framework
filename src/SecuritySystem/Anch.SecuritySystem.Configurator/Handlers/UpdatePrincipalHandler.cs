using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Anch.SecuritySystem.ExternalSystem.Management;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class UpdatePrincipalHandler(
    [WithoutRunAs] ISecuritySystem securitySystem,
    IPrincipalManagementService principalManagementService)
    : BaseWriteHandler, IUpdatePrincipalHandler
{
    public async Task Execute(HttpContext context, CancellationToken ct)
    {
        await securitySystem.CheckAccessAsync(ApplicationSecurityRule.SecurityAdministrator, ct);

        var principalName = await this.ParseRequestBodyAsync<string>(context);

        await principalManagementService.UpdatePrincipalNameAsync(context.ExtractSecurityIdentity(), principalName, ct);
    }
}