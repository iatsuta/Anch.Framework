using Anch.Core;
using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Testing;

public class RootUserCredentialManager(
    AdministratorsRoleList administratorsRoleList,
    ITestingEvaluator<UserCredentialManager> baseEvaluator,
    RootImpersonateServiceState rootImpersonateServiceState,
    Tuple<UserCredential?> userCredential,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null)
{
    private ITestingEvaluator<UserCredentialManager> ManagerEvaluator { get; } =

        baseEvaluator.Select(service => userCredential.Item1 == null ? service : service.ReplaceCurrentUser(userCredential.Item1));

    public void LoginAs() =>

        rootImpersonateServiceState.CustomUserCredential = userCredential.Item1;

    public SecurityIdentity CreatePrincipal() =>

        defaultCancellationTokenSource.RunSync(async ct => await this.CreatePrincipalAsync(ct));

    public Task<SecurityIdentity> CreatePrincipalAsync(CancellationToken cancellationToken = default) =>

        this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Write, manager => manager.CreatePrincipalAsync(cancellationToken));

    public SecurityIdentity SetAdminRole() =>

        defaultCancellationTokenSource.RunSync(async ct => await this.SetAdminRoleAsync(ct));

    public Task<SecurityIdentity> SetAdminRoleAsync(CancellationToken cancellationToken = default) =>

        this.SetRoleAsync(administratorsRoleList.Roles.Select(securityRole => (ManagedPermission)securityRole).ToArray(), cancellationToken);

    public SecurityIdentity SetRole(params ManagedPermission[] permissions) =>

        defaultCancellationTokenSource.RunSync(async ct => await this.SetRoleAsync(permissions, ct));

    public Task<SecurityIdentity> SetRoleAsync(ManagedPermission permission, CancellationToken cancellationToken = default) =>

        this.SetRoleAsync([permission], cancellationToken);

    public async Task<SecurityIdentity> SetRoleAsync(ManagedPermission[] permissions, CancellationToken cancellationToken = default)
    {
        await this.ClearRolesAsync(cancellationToken);

        return await this.AddRoleAsync(permissions, cancellationToken);
    }

    public SecurityIdentity AddRole(params ManagedPermission[] permissions) =>

        defaultCancellationTokenSource.RunSync(async ct => await this.AddRoleAsync(permissions, ct));

    public Task<SecurityIdentity> AddRoleAsync(ManagedPermission permission, CancellationToken cancellationToken = default) =>

        this.AddRoleAsync([permission], cancellationToken);

    public Task<SecurityIdentity> AddRoleAsync(ManagedPermission[] permissions, CancellationToken cancellationToken = default) =>

        this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Write, manager => manager.AddUserRoleAsync(permissions, cancellationToken));

    public void ClearRoles() =>

        defaultCancellationTokenSource.RunSync(async ct => await this.ClearRolesAsync(ct));

    public Task ClearRolesAsync(CancellationToken cancellationToken = default) =>

        this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Write, manager => manager.RemovePermissionsAsync(cancellationToken));

    public ManagedPrincipal GetPrincipal() =>

        defaultCancellationTokenSource.RunSync(async ct => await this.GetPrincipalAsync(ct));

    public Task<ManagedPrincipal> GetPrincipalAsync(CancellationToken cancellationToken = default) =>

        this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Read, manager => manager.GetPrincipalAsync(cancellationToken));
}