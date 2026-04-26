using Anch.Core;
using Anch.SecuritySystem.Attributes;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.Services;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class UpdatePermissionsHandler(
    [WithoutRunAs] ISecuritySystem securitySystem,
    ISecurityRoleSource securityRoleSource,
    ISecurityContextInfoSource securityContextInfoSource,
    IDomainObjectIdentsParser domainObjectIdentsParser,
	IPrincipalManagementService principalManagementService,
    IConfiguratorIntegrationEvents? configuratorIntegrationEvents = null) : BaseWriteHandler, IUpdatePermissionsHandler
{
    public async Task Execute(HttpContext context, CancellationToken cancellationToken)
    {
        await securitySystem.CheckAccessAsync(ApplicationSecurityRule.SecurityAdministrator, cancellationToken);

        var permissions = await this.ParseRequestBodyAsync<List<RequestBodyDto>>(context);

        var managedPermissions = permissions.Select(this.ToManagedPermission).ToList();

        var mergeResult = await principalManagementService.UpdatePermissionsAsync(context.ExtractSecurityIdentity(), managedPermissions, cancellationToken);

        if (configuratorIntegrationEvents != null)
        {
            foreach (var permission in mergeResult.AddingItems)
            {
                await configuratorIntegrationEvents.PermissionCreatedAsync(permission, cancellationToken);
            }

            foreach (var (permission, _) in mergeResult.CombineItems)
            {
                await configuratorIntegrationEvents.PermissionChangedAsync(permission, cancellationToken);
            }

            foreach (var permission in mergeResult.RemovingItems)
            {
                await configuratorIntegrationEvents.PermissionRemovedAsync(permission, cancellationToken);
            }
        }
    }

    private ManagedPermission ToManagedPermission(RequestBodyDto permission)
    {
        var restrictionsRequest =

            from restriction in permission.Contexts

            let securityContextType = securityContextInfoSource.GetSecurityContextInfo(new UntypedSecurityIdentity(restriction.Id)).Type

            let idents = domainObjectIdentsParser.Parse(securityContextType, restriction.Entities)

            select (securityContextType, idents);

        return new()
        {
            Identity = new UntypedSecurityIdentity(permission.PermissionId),
            IsVirtual = permission.IsVirtual,
            SecurityRole = securityRoleSource.GetSecurityRole(new UntypedSecurityIdentity(permission.RoleId)),
            Period = new PermissionPeriod(permission.StartDate, permission.EndDate),
            Comment = permission.Comment,
            DelegatedFrom = new UntypedSecurityIdentity(permission.DelegatedFromId),
            Restrictions = restrictionsRequest.ToImmutableDictionary()
        };
    }

    private class RequestBodyDto
    {
        public required string PermissionId { get; set; }

        public required string RoleId { get; set; }

        public bool IsVirtual { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Comment { get; set; } = default!;

        public List<ContextDto> Contexts { get; set; } = default!;

        public string DelegatedFromId { get; set; } = default!;

        public class ContextDto
        {
            public string Id { get; set; } = default!;

            public List<string> Entities { get; set; } = default!;
        }
    }
}
