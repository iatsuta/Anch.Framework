using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Configurator.Models;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Anch.SecuritySystem.ExternalSystem.Management;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class GetPrincipalsHandler([WithoutRunAs] ISecuritySystem securitySystem, IRootPrincipalSourceService principalSourceService)
    : BaseReadHandler, IGetPrincipalsHandler
{
    protected override async Task<object> GetDataAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!await securitySystem.IsSecurityAdministratorAsync(cancellationToken))
        {
            return Array.Empty<EntityDto>();
        }

        var nameFilter = context.ExtractSearchToken();

        return await principalSourceService
            .GetPrincipalsAsync(nameFilter, 70)
            .Select(x => new PrincipalHeaderDto { Id = x.Identity.GetId().ToString()!, Name = x.Name, IsVirtual = x.IsVirtual })
            .OrderBy(x => x.Name)
            .ToArrayAsync(cancellationToken);
    }
}