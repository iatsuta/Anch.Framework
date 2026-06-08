using Anch.Core;
using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Testing;

public class RootUserCredentialManager(
    AdministratorsRoleList administratorsRoleList,
    ITestingEvaluator<IServiceProxyFactory> baseEvaluator,
    RootImpersonateServiceState rootImpersonateServiceState,
    Tuple<UserCredential?> userCredential,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null)
{
    private ITestingEvaluator<UserCredentialManager> ManagerEvaluator { get; } =
        baseEvaluator.Select(service => service.Create<UserCredentialManager>(userCredential));

    public void LoginAs() =>

        rootImpersonateServiceState.CustomUserCredential = userCredential.Item1;

    public SecurityIdentity CreatePrincipal() =>

        defaultCancellationTokenSource.RunSync(async ct => await this.CreatePrincipalAsync(ct));

    public Task<SecurityIdentity> CreatePrincipalAsync(CancellationToken ct) =>

        this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Write, manager => manager.CreatePrincipalAsync(ct));

    public SecurityIdentity SetAdminRole() =>

        defaultCancellationTokenSource.RunSync(async ct => await this.SetAdminRoleAsync(ct));

    public Task<SecurityIdentity> SetAdminRoleAsync(CancellationToken ct) =>

        this.SetRoleAsync(administratorsRoleList.Roles.Select(securityRole => (ManagedPermission)securityRole).ToArray(), ct);

    public SecurityIdentity SetRole(params ManagedPermission[] permissions) =>

        defaultCancellationTokenSource.RunSync(async ct => await this.SetRoleAsync(permissions, ct));

    public Task<SecurityIdentity> SetRoleAsync(ManagedPermission permission, CancellationToken ct) =>

        this.SetRoleAsync([permission], ct);

    public async Task<SecurityIdentity> SetRoleAsync(ManagedPermission[] permissions, CancellationToken ct)
    {
        await this.ClearRolesAsync(ct);

        return await this.AddRoleAsync(permissions, ct);
    }

    public SecurityIdentity AddRole(params ManagedPermission[] permissions) =>

        defaultCancellationTokenSource.RunSync(async ct => await this.AddRoleAsync(permissions, ct));

    public Task<SecurityIdentity> AddRoleAsync(ManagedPermission permission, CancellationToken ct) =>

        this.AddRoleAsync([permission], ct);

    public Task<SecurityIdentity> AddRoleAsync(ManagedPermission[] permissions, CancellationToken ct) =>

        this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Write, manager => manager.AddUserRoleAsync(permissions, ct));

    public void ClearRoles() =>

        defaultCancellationTokenSource.RunSync(async ct => await this.ClearRolesAsync(ct));

    public Task ClearRolesAsync(CancellationToken ct) =>

        this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Write, manager => manager.RemovePermissionsAsync(ct));

    public ManagedPrincipal GetPrincipal() =>

        defaultCancellationTokenSource.RunSync(async ct => await this.GetPrincipalAsync(ct));

    public Task<ManagedPrincipal> GetPrincipalAsync(CancellationToken ct) =>

        this.ManagerEvaluator.EvaluateAsync(TestingScopeMode.Read, manager => manager.GetPrincipalAsync(ct));
}