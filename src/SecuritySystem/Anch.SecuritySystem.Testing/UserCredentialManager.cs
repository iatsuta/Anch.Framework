using Anch.Core.Auth;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Testing;

public class UserCredentialManager(
    ICurrentUser currentUser,
    Tuple<UserCredential?> userCredential,
    IPrincipalManagementService principalManagementService,
    IRootPrincipalSourceService rootPrincipalSourceService,
    IPrincipalDataSecurityIdentityManager securityIdentityManager)
{
    private readonly IPrincipalSourceService principalSourceService = rootPrincipalSourceService.ForPrincipal(principalManagementService.PrincipalType);

    private UserCredential ActualCredential => userCredential.Item1 ?? currentUser.Name;

    public async Task<SecurityIdentity> CreatePrincipalAsync(CancellationToken ct)
    {
        var principalData = await principalManagementService.CreatePrincipalAsync(this.ActualCredential, [], ct);

        return securityIdentityManager.Extract(principalData);
    }

    public async Task<SecurityIdentity> AddUserRoleAsync(ManagedPermission[] newPermissions, CancellationToken ct)
    {
        var existsPrincipal = await this.principalSourceService.TryGetPrincipalAsync(this.ActualCredential, ct);

        if (existsPrincipal == null)
        {
            var newPrincipalData = await principalManagementService.CreatePrincipalAsync(this.ActualCredential, newPermissions, ct);

            return securityIdentityManager.Extract(newPrincipalData);
        }
        else
        {
            var updatedPrincipal = existsPrincipal with { Permissions = [.. existsPrincipal.Permissions, .. newPermissions] };

            await principalManagementService.UpdatePermissionsAsync(
                updatedPrincipal.Header.Identity,
                updatedPrincipal.Permissions,
                ct);

            return updatedPrincipal.Header.Identity;
        }
    }

    public async Task RemovePermissionsAsync(CancellationToken ct)
    {
        var principal = await this.principalSourceService.TryGetPrincipalAsync(this.ActualCredential, ct);

        if (principal is { Header.IsVirtual: false })
        {
            await principalManagementService.RemovePrincipalAsync(principal.Header.Identity, true, ct);
        }
    }

    public async Task<ManagedPrincipal> GetPrincipalAsync(CancellationToken ct)
    {
        return await this.principalSourceService.GetPrincipalAsync(this.ActualCredential, ct);
    }
}