using Anch.Core;
using Anch.GenericRepository;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.GeneralPermission.Validation.Principal;
using Anch.SecuritySystem.Services;
using Anch.SecuritySystem.UserSource;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.GeneralPermission;

public class GeneralPrincipalManagementService<TPrincipal, TPermission, TPermissionRestriction>(
    IPrincipalValidator<TPrincipal, TPermission, TPermissionRestriction> principalValidator,
    IGenericRepository genericRepository,
    IPrincipalDomainService<TPrincipal> principalDomainService,
    IUserSource<TPrincipal> principalUserSource,
    ISecurityIdentityManager<TPermission> permissionIdentityManager,
    IPermissionLoader<TPrincipal, TPermission> permissionLoader,
    IPermissionRestrictionLoader<TPermission, TPermissionRestriction> permissionRestrictionLoader,
    IPermissionManagementService<TPrincipal, TPermission, TPermissionRestriction> permissionManagementService,
    IVisualIdentityInfo<TPrincipal> principalVisualIdentityInfo)
    : IPrincipalManagementService

    where TPrincipal : class
    where TPermission : class
    where TPermissionRestriction : class
{
    public Type PrincipalType { get; } = typeof(TPrincipal);

    public async Task<PrincipalData> CreatePrincipalAsync(
        UserCredential userCredential,
        IEnumerable<ManagedPermission> managedPermissions,
        CancellationToken ct)
    {
        var principal = await principalDomainService.GetOrCreateAsync(userCredential, ct);

        var result = await this.UpdatePermissionsAsync(principal, [], managedPermissions, ct);

        return new PrincipalData<TPrincipal, TPermission, TPermissionRestriction>(principal,
            [.. result.AddingItems.Cast<PermissionData<TPermission, TPermissionRestriction>>()]);
    }

    public async Task<PrincipalData> UpdatePrincipalNameAsync(
        UserCredential userCredential,
        string principalName,
        CancellationToken ct)
    {
        var principal = await principalUserSource.GetUserAsync(userCredential, ct);

        principalVisualIdentityInfo.Name.Setter(principal, principalName);

        await genericRepository.SaveAsync(principal, ct);

        return await this.ToPrincipalData(principal, ct);
    }

    public async Task<PrincipalData> RemovePrincipalAsync(UserCredential userCredential, bool force, CancellationToken ct)
    {
        var principal = await principalUserSource.GetUserAsync(userCredential, ct);

        var principalData = await this.ToPrincipalData(principal, ct);

        await principalDomainService.RemoveAsync(principal, force, ct);

        return principalData;
    }

    private async Task<PrincipalData<TPrincipal, TPermission, TPermissionRestriction>> ToPrincipalData(TPrincipal dbPrincipal,
        CancellationToken ct)
    {
        var permissionsData = await permissionLoader
            .LoadAsync(dbPrincipal)
            .Select(async (v, lct) => await permissionRestrictionLoader.ToPermissionData(v, lct))
            .ToImmutableArrayAsync(ct);

        return new PrincipalData<TPrincipal, TPermission, TPermissionRestriction>(dbPrincipal, permissionsData);
    }

    public async Task<MergeResult<PermissionData, PermissionData>> UpdatePermissionsAsync(
        UserCredential userCredential,
        IEnumerable<ManagedPermission> managedPermissions,
        CancellationToken ct)
    {
        var dbPrincipal = await principalUserSource.GetUserAsync(userCredential, ct);

        var dbPermissions = await permissionLoader.LoadAsync(dbPrincipal).ToArrayAsync(ct);

        return await this.UpdatePermissionsAsync(dbPrincipal, dbPermissions, managedPermissions, ct);
    }

    private async Task<MergeResult<PermissionData, PermissionData>> UpdatePermissionsAsync(
        TPrincipal dbPrincipal,
        TPermission[] dbPermissions,
        IEnumerable<ManagedPermission> managedPermissions,
        CancellationToken ct)
    {
        var permissionMergeResult = dbPermissions.GetMergeResult(managedPermissions, permissionIdentityManager.GetIdentity,
            p => p.Identity.IsDefault ? new object() : permissionIdentityManager.Converter.Convert(p.Identity));

        var newPermissions = await permissionMergeResult
            .AddingItems
            .ToAsyncEnumerable()
            .Select(async (managedPermission, lct) => await permissionManagementService.CreatePermissionAsync(dbPrincipal, managedPermission, lct))
            .ToArrayAsync(ct);

        var updatedPermissions = await permissionMergeResult
            .CombineItems
            .ToAsyncEnumerable()
            .Select(async (permissionPair, lct) => await permissionManagementService.UpdatePermission(permissionPair.Item1, permissionPair.Item2, lct))
            .ToArrayAsync(ct);

        var removingPermissions = await permissionMergeResult
            .RemovingItems
            .ToAsyncEnumerable()
            .Select(async (oldDbPermission, lct) =>
            {
                var result = await permissionRestrictionLoader.ToPermissionData(oldDbPermission, lct);

                foreach (var dbRestriction in result.Restrictions)
                {
                    await genericRepository.RemoveAsync(dbRestriction, lct);
                }

                await genericRepository.RemoveAsync(oldDbPermission, lct);

                return result;
            }).ToArrayAsync(ct);

        await principalValidator.ValidateAsync(
            new PrincipalData<TPrincipal, TPermission, TPermissionRestriction>(
                dbPrincipal,
                [.. updatedPermissions.Select(pair => pair.PermissonData), .. newPermissions]),
            ct);

        return new MergeResult<PermissionData, PermissionData>(
            newPermissions,
            updatedPermissions.Where(pair => pair.Updated).Select(PermissionData (pair) => pair.PermissonData).Select(v => (v, v)),
            removingPermissions);
    }
}