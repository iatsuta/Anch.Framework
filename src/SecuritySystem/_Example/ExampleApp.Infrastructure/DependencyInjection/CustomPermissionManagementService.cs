using Anch.GenericRepository;
using Anch.SecuritySystem;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.GeneralPermission;
using Anch.SecuritySystem.Services;
using AuthGeneral = ExampleApp.Domain.Auth.General;

namespace ExampleApp.Infrastructure.DependencyInjection;

public class CustomPermissionManagementService(
    PermissionBindingInfo<AuthGeneral.Permission, AuthGeneral.Principal> bindingInfo,
    GeneralPermissionBindingInfo<AuthGeneral.Permission, AuthGeneral.SecurityRole> generalBindingInfo,
    GeneralPermissionRestrictionBindingInfo<AuthGeneral.PermissionRestriction, AuthGeneral.SecurityContextType, Guid, AuthGeneral.Permission>
        restrictionBindingInfo,
    IPermissionSecurityRoleResolver<AuthGeneral.Permission> permissionSecurityRoleResolver,
    IRawPermissionRestrictionLoader<AuthGeneral.Permission> rawPermissionRestrictionLoader,
    ISecurityIdentityManager<AuthGeneral.Permission> permissionSecurityIdentityManager,
    ISecurityRepository<AuthGeneral.SecurityRole> securityRoleRepository,
    IQueryableSource queryableSource,
    ISecurityRoleSource securityRoleSource,
    ISecurityRepository<AuthGeneral.SecurityContextType> securityContextTypeRepository,
    ISecurityContextInfoSource securityContextInfoSource,
    ISecurityIdentityManager<AuthGeneral.SecurityRole> securityRoleIdentityManager,
    ISecurityIdentityManager<AuthGeneral.SecurityContextType> securityContextTypeIdentityManager,
    ISecurityIdentityManager<AuthGeneral.Permission> permissionIdentityManager,
    IGenericRepository genericRepository,
    ISecurityRepository<AuthGeneral.Permission> permissionRepository) :
    PermissionManagementService<AuthGeneral.Principal, AuthGeneral.Permission, AuthGeneral.SecurityRole, AuthGeneral.PermissionRestriction,
        AuthGeneral.SecurityContextType, Guid>(bindingInfo, generalBindingInfo, restrictionBindingInfo, permissionSecurityRoleResolver,
        rawPermissionRestrictionLoader, permissionSecurityIdentityManager, securityRoleRepository, queryableSource, securityRoleSource,
        securityContextTypeRepository, securityContextInfoSource, securityRoleIdentityManager, securityContextTypeIdentityManager, permissionIdentityManager,
        genericRepository, permissionRepository)

{
    private const string ExtendedKey = nameof(AuthGeneral.Permission.ExtendedValue);

    public override async ValueTask<PermissionData<AuthGeneral.Permission, AuthGeneral.PermissionRestriction>> CreatePermissionAsync(
        AuthGeneral.Principal dbPrincipal,
        ManagedPermission managedPermission,
        CancellationToken cancellationToken)
    {
        var baseResult = await base.CreatePermissionAsync(dbPrincipal, managedPermission, cancellationToken);

        if (managedPermission.ExtendedData.TryGetValue(ExtendedKey, out var extendedValue))
        {
            baseResult.Permission.ExtendedValue = (string)extendedValue;

            await genericRepository.SaveAsync(baseResult.Permission, cancellationToken);
        }

        return baseResult;
    }

    public override async ValueTask<ManagedPermission> ToManagedPermissionAsync(AuthGeneral.Permission dbPermission, CancellationToken cancellationToken)
    {
        var baseResult = await base.ToManagedPermissionAsync(dbPermission, cancellationToken);

        return baseResult.WithExtendedData(ExtendedKey, dbPermission.ExtendedValue);
    }

    public override async ValueTask<(PermissionData<AuthGeneral.Permission, AuthGeneral.PermissionRestriction> PermissonData, bool Updated)> UpdatePermission(
        AuthGeneral.Permission dbPermission, ManagedPermission managedPermission, CancellationToken cancellationToken)
    {
        var baseResult = await base.UpdatePermission(dbPermission, managedPermission, cancellationToken);

        if (managedPermission.ExtendedData.TryGetValue(ExtendedKey, out var extendedValue) && (string)extendedValue != dbPermission.ExtendedValue)
        {
            throw new SecuritySystemException($"{ExtendedKey} can't be changed");
        }

        return baseResult;
    }
}