using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Configurator.Models;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class GetBusinessRoleContextsHandler(
    ISecurityContextInfoSource securityContextInfoSource,
    [WithoutRunAs] ISecuritySystem securitySystem)
    : BaseReadHandler, IGetBusinessRoleContextsHandler
{
    protected override async Task<object> GetDataAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!await securitySystem.IsSecurityAdministratorAsync(cancellationToken))
        {
            return Array.Empty<EntityDto>();
        }
        else
        {
            return await securityContextInfoSource
                .SecurityContextInfoList
                .ToAsyncEnumerable()
                .Select(x => new EntityDto { Id = x.Identity.GetId().ToString()!, Name = x.Name })
                .OrderBy(x => x.Name)
                .ToArrayAsync(cancellationToken);
        }
    }
}