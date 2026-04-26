using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Anch.SecuritySystem.ExternalSystem.Management;
using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class UpdatePrincipalHandler(
	[WithoutRunAs] ISecuritySystem securitySystem,
	IPrincipalManagementService principalManagementService,
	IConfiguratorIntegrationEvents? configuratorIntegrationEvents = null)
	: BaseWriteHandler, IUpdatePrincipalHandler
{
	public async Task Execute(HttpContext context, CancellationToken cancellationToken)
	{
		await securitySystem.CheckAccessAsync(ApplicationSecurityRule.SecurityAdministrator, cancellationToken);

		var principalName = await this.ParseRequestBodyAsync<string>(context);

		var principal = await principalManagementService.UpdatePrincipalNameAsync(context.ExtractSecurityIdentity(), principalName,
			cancellationToken);

		if (configuratorIntegrationEvents != null)
			await configuratorIntegrationEvents.PrincipalChangedAsync(principal, cancellationToken);
	}
}