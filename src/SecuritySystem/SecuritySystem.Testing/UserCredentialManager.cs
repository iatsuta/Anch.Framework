using CommonFramework.Auth;
using SecuritySystem.ExternalSystem.Management;
using SecuritySystem.Services;

namespace SecuritySystem.Testing;

public class UserCredentialManager(
    ICurrentUser currentUser,
    IPrincipalManagementService principalManagementService,
    IRootPrincipalSourceService rootPrincipalSourceService,
    ISyncUserNameResolver userNameResolver,
    IPrincipalDataSecurityIdentityManager securityIdentityManager)
{
    private readonly IPrincipalSourceService principalSourceService = rootPrincipalSourceService.ForPrincipal(principalManagementService.PrincipalType);

    public UserCredentialManager ReplaceCurrentUser(UserCredential userCredential)
    {
        return new UserCredentialManager(
            new FixedCurrentUser(userNameResolver.GetUserName(userCredential)),
            principalManagementService,
            rootPrincipalSourceService,
            userNameResolver,
            securityIdentityManager);
    }

    public async Task<SecurityIdentity> CreatePrincipalAsync(CancellationToken cancellationToken = default)
    {
        var principalData = await principalManagementService.CreatePrincipalAsync(currentUser.Name, [], cancellationToken);

        return securityIdentityManager.Extract(principalData);
    }

    public async Task<SecurityIdentity> AddUserRoleAsync(ManagedPermission[] newPermissions, CancellationToken cancellationToken = default)
    {
        var existsPrincipal = await principalSourceService.TryGetPrincipalAsync(currentUser.Name, cancellationToken);

        if (existsPrincipal == null)
        {
            var newPrincipalData = await principalManagementService.CreatePrincipalAsync(currentUser.Name, newPermissions, cancellationToken);

            return securityIdentityManager.Extract(newPrincipalData);
        }
        else
        {
            var updatedPrincipal = existsPrincipal with { Permissions = [..existsPrincipal.Permissions, .. newPermissions] };

            await principalManagementService.UpdatePermissionsAsync(
                updatedPrincipal.Header.Identity,
                updatedPrincipal.Permissions,
                cancellationToken);

            return updatedPrincipal.Header.Identity;
        }
    }

    public async Task RemovePermissionsAsync(CancellationToken cancellationToken = default)
    {
        var principal = await principalSourceService.TryGetPrincipalAsync(currentUser.Name, cancellationToken);

        if (principal is { Header.IsVirtual: false })
        {
            await principalManagementService.RemovePrincipalAsync(principal.Header.Identity, true, cancellationToken);
        }
    }

    public async Task<ManagedPrincipal> GetPrincipalAsync(CancellationToken cancellationToken = default)
    {
        return await principalSourceService.GetPrincipalAsync(currentUser.Name, cancellationToken);
    }
}