using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Configurator.Models;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Anch.SecuritySystem.ExternalSystem.SecurityContextStorage;
using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class GetBusinessRoleContextEntitiesHandler(
    ISecurityContextStorage securityContextStorage,
    ISecurityContextInfoSource securityContextInfoSource,
    [WithoutRunAs] ISecuritySystem securitySystem)
    : BaseReadHandler, IGetBusinessRoleContextEntitiesHandler
{
    protected override async Task<object> GetDataAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!await securitySystem.IsSecurityAdministratorAsync(cancellationToken))
        {
            return Array.Empty<EntityDto>();
        }
        else
        {
            var securityContextType = securityContextInfoSource.GetSecurityContextInfo(context.ExtractSecurityIdentity()).Type;

            var searchToken = context.ExtractSearchToken();

            var typedSecurityContextStorage = securityContextStorage.GetTyped(securityContextType);

            var entities = typedSecurityContextStorage.GetSecurityContexts();

            if (!string.IsNullOrWhiteSpace(searchToken))
                entities = entities.Where(p => p.Name.Contains(searchToken, StringComparison.OrdinalIgnoreCase));

            return await entities
                .ToAsyncEnumerable()
                .Select(x => new RestrictionDto { Id = x.Id.ToString()!, Name = x.Name })
                .OrderByDescending(x => x.Name.Equals(searchToken, StringComparison.OrdinalIgnoreCase))
                .ThenBy(x => x.Name)
                .Take(70)
                .ToArrayAsync(cancellationToken);
        }
    }
}