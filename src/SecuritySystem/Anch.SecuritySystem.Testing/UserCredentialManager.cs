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

    public async Task<SecurityIdentity> CreatePrincipalAsync(CancellationToken cancellationToken = default)
    {
        var principalData = await principalManagementService.CreatePrincipalAsync(this.ActualCredential, [], cancellationToken);

        return securityIdentityManager.Extract(principalData);
    }

    public async Task<SecurityIdentity> AddUserRoleAsync(ManagedPermission[] newPermissions, CancellationToken cancellationToken = default)
    {
        var existsPrincipal = await this.principalSourceService.TryGetPrincipalAsync(this.ActualCredential, cancellationToken);

        if (existsPrincipal == null)
        {
            var newPrincipalData = await principalManagementService.CreatePrincipalAsync(this.ActualCredential, newPermissions, cancellationToken);

            return securityIdentityManager.Extract(newPrincipalData);
        }
        else
        {
            var updatedPrincipal = existsPrincipal with { Permissions = [.. existsPrincipal.Permissions, .. newPermissions] };

            await principalManagementService.UpdatePermissionsAsync(
                updatedPrincipal.Header.Identity,
                updatedPrincipal.Permissions,
                cancellationToken);

            return updatedPrincipal.Header.Identity;
        }
    }

    public async Task RemovePermissionsAsync(CancellationToken cancellationToken = default)
    {
        var principal = await this.principalSourceService.TryGetPrincipalAsync(this.ActualCredential, cancellationToken);

        if (principal is { Header.IsVirtual: false })
        {
            await principalManagementService.RemovePrincipalAsync(principal.Header.Identity, true, cancellationToken);
        }
    }

    public async Task<ManagedPrincipal> GetPrincipalAsync(CancellationToken cancellationToken = default)
    {
        return await this.principalSourceService.GetPrincipalAsync(this.ActualCredential, cancellationToken);
    }
}