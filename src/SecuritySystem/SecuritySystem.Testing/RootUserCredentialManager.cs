using CommonFramework;
using SecuritySystem.ExternalSystem.Management;

namespace SecuritySystem.Testing;

public class RootUserCredentialManager(
    AdministratorsRoleList administratorsRoleList,
    ITestingEvaluator<UserCredentialManager> baseEvaluator,
    RootImpersonateServiceState rootImpersonateServiceState,
    Tuple<UserCredential?> userCredential,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null)
{
    private ITestingEvaluator<UserCredentialManager> ManagerEvaluator { get; } =
        baseEvaluator.Select(service => userCredential.Item1 == null ? service : service.ReplaceCurrentUser(userCredential.Item1));

    public void LoginAs()
    {
        rootImpersonateServiceState.CustomUserCredential = userCredential.Item1;
    }

    public SecurityIdentity CreatePrincipal()
    {
        return defaultCancellationTokenSource.RunSync(async ct => await this.CreatePrincipalAsync(ct));
    }

    public async Task<SecurityIdentity> CreatePrincipalAsync(CancellationToken cancellationToken = default)
    {
        return await this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Write, manager => manager.CreatePrincipalAsync(cancellationToken));
    }

    public SecurityIdentity SetAdminRole()
    {
        return defaultCancellationTokenSource.RunSync(async ct => await this.SetAdminRoleAsync(ct));
    }

    public Task<SecurityIdentity> SetAdminRoleAsync(CancellationToken cancellationToken = default)
    {
        return this.SetRoleAsync(administratorsRoleList.Roles.Select(securityRole => (ManagedPermission)securityRole).ToArray(),
            cancellationToken);
    }

    public SecurityIdentity SetRole(params ManagedPermission[] permissions)
    {
        return defaultCancellationTokenSource.RunSync(async ct => await this.SetRoleAsync(permissions, ct));
    }

    public async Task<SecurityIdentity> SetRoleAsync(ManagedPermission permission, CancellationToken cancellationToken = default)
    {
        return await this.SetRoleAsync([permission], cancellationToken);
    }

    public async Task<SecurityIdentity> SetRoleAsync(ManagedPermission[] permissions, CancellationToken cancellationToken = default)
    {
        await this.ClearRolesAsync(cancellationToken);

        return await this.AddRoleAsync(permissions, cancellationToken);
    }

    public SecurityIdentity AddRole(params ManagedPermission[] permissions) =>
        defaultCancellationTokenSource.RunSync(async ct => await this.AddRoleAsync(permissions, ct));

    public async Task<SecurityIdentity> AddRoleAsync(ManagedPermission permission, CancellationToken cancellationToken = default) =>
        await this.AddRoleAsync([permission], cancellationToken);

    public async Task<SecurityIdentity> AddRoleAsync(ManagedPermission[] permissions, CancellationToken cancellationToken = default) =>
        await this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Write, manager => manager.AddUserRoleAsync(permissions, cancellationToken));

    public void ClearRoles()
    {
        defaultCancellationTokenSource.RunSync(async ct => await this.ClearRolesAsync(ct));
    }

    public async Task ClearRolesAsync(CancellationToken cancellationToken = default)
    {
        await this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Write, manager => manager.RemovePermissionsAsync(cancellationToken));
    }

    public ManagedPrincipal GetPrincipal()
    {
        return defaultCancellationTokenSource.RunSync(async ct => await this.GetPrincipalAsync(ct));
    }

    public async Task<ManagedPrincipal> GetPrincipalAsync(CancellationToken cancellationToken = default)
    {
        return await this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Read, manager => manager.GetPrincipalAsync(cancellationToken));
    }
}