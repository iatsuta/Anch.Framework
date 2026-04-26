using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Configurator.Models;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class GetBusinessRolesHandler(
    ISecurityRoleSource securityRoleSource,
    ISecurityContextInfoSource securityContextInfoSource,
    [WithoutRunAs] ISecuritySystem securitySystem)
    : BaseReadHandler, IGetBusinessRolesHandler
{
    protected override async Task<object> GetDataAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!await securitySystem.IsSecurityAdministratorAsync(cancellationToken))
        {
            return Array.Empty<EntityDto>();
        }
        else
        {
            var defaultContexts = securityContextInfoSource.SecurityContextInfoList
                .Select(v => new RoleContextDto(v.Name, false))
                .ToList();

            return await securityRoleSource
                .SecurityRoles
                .ToAsyncEnumerable()
                .Select(x => new FullRoleDto
                {
                    Id = x.Identity.GetId().ToString()!,
                    Name = x.Name,
                    IsVirtual = x.Information.IsVirtual,
                    Contexts =
                        x.Information.Restriction.SecurityContextRestrictions?.Select(v =>
                            new RoleContextDto(securityContextInfoSource.GetSecurityContextInfo(v.SecurityContextType).Name, v.Required)).ToList()
                        ?? defaultContexts
                })
                .OrderBy(x => x.Name)
                .ToArrayAsync(cancellationToken);
        }
    }
}