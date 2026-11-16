using Anch.SecuritySystem.ExternalSystem.Management;

using ExampleApp.Domain.Auth.General;

namespace ExampleApp.Infrastructure.Services;

public class ExamplePrincipalManagementListener(ExamplePrincipalManagementListenerState state) : IPrincipalManagementListener<Principal, Permission, PermissionRestriction>
{
    public async Task PrincipalCreatedAsync(PrincipalData<Principal, Permission, PermissionRestriction> principalData, CancellationToken ct)
    {
        state.CreatedPrincipals.Add(principalData.Principal.Id);
    }

    public async Task PrincipalChangedAsync(PrincipalData<Principal, Permission, PermissionRestriction> principalData, CancellationToken ct)
    {

    }

    public async Task PrincipalRemovedAsync(PrincipalData<Principal, Permission, PermissionRestriction> principalData, CancellationToken ct)
    {

    }

    public async Task PermissionCreatedAsync(PermissionData<Permission, PermissionRestriction> permissionData, CancellationToken ct)
    {

    }

    public async Task PermissionChangedAsync(PermissionData<Permission, PermissionRestriction> permissionData, CancellationToken ct)
    {

    }

    public async Task PermissionRemovedAsync(PermissionData<Permission, PermissionRestriction> permissionData, CancellationToken ct)
    {

    }
}