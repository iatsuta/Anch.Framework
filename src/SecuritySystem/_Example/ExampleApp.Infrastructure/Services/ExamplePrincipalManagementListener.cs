using Anch.SecuritySystem.ExternalSystem.Management;

using ExampleApp.Domain.Auth.General;

namespace ExampleApp.Infrastructure.Services;

public class ExamplePrincipalManagementListener(ExamplePrincipalManagementListenerState state) : IPrincipalManagementListener<Principal, Permission, PermissionRestriction>
{
    public async Task PrincipalCreatedAsync(PrincipalData<Principal, Permission, PermissionRestriction> principal, CancellationToken ct)
    {
        state.CreatedPrincipals.Add(principal.Principal.Id);
    }

    public async Task PrincipalChangedAsync(PrincipalData<Principal, Permission, PermissionRestriction> principal, CancellationToken ct)
    {

    }

    public async Task PrincipalRemovedAsync(PrincipalData<Principal, Permission, PermissionRestriction> principal, CancellationToken ct)
    {

    }

    public async Task PermissionCreatedAsync(PermissionData<Permission, PermissionRestriction> permission, CancellationToken ct)
    {

    }

    public async Task PermissionChangedAsync(PermissionData<Permission, PermissionRestriction> permission, CancellationToken ct)
    {

    }

    public async Task PermissionRemovedAsync(PermissionData<Permission, PermissionRestriction> permission, CancellationToken ct)
    {

    }
}