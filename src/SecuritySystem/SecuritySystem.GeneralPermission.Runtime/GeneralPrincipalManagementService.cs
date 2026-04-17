using CommonFramework;
using CommonFramework.GenericRepository;
using CommonFramework.VisualIdentitySource;
using SecuritySystem.ExternalSystem.Management;
using SecuritySystem.GeneralPermission.Validation.Principal;
using SecuritySystem.Services;
using SecuritySystem.UserSource;

namespace SecuritySystem.GeneralPermission;

public class GeneralPrincipalManagementService(
    IServiceProxyFactory serviceProxyFactory,
    IEnumerable<PermissionBindingInfo> bindingInfoList,
    IGeneralPermissionRestrictionBindingInfoSource restrictionBindingInfoSource)
    : IPrincipalManagementService
{
    private readonly Lazy<IPrincipalManagementService> lazyInnerService = new(() =>
    {
        var bindingInfo = bindingInfoList.Single(
            bi => !bi.IsReadonly,
            () => new SecuritySystemException("No writable management service was found"),
            () => new SecuritySystemException("Multiple writable management services were found"));

        var restrictionBindingInfo = restrictionBindingInfoSource.GetForPermission(bindingInfo.PermissionType);

        var innerServiceType = typeof(GeneralPrincipalManagementService<,,>)
            .MakeGenericType(bindingInfo.PrincipalType, bindingInfo.PermissionType, restrictionBindingInfo.PermissionRestrictionType);

        return serviceProxyFactory.Create<IPrincipalManagementService>(innerServiceType);
    });

    private IPrincipalManagementService InnerService => this.lazyInnerService.Value;

    public Type PrincipalType => this.InnerService.PrincipalType;

    public Task<PrincipalData> CreatePrincipalAsync(string principalName, IEnumerable<ManagedPermission> managedPermissions, CancellationToken cancellationToken = default) =>
        this.InnerService.CreatePrincipalAsync(principalName, managedPermissions, cancellationToken);

    public Task<PrincipalData> UpdatePrincipalNameAsync(UserCredential userCredential, string principalName, CancellationToken cancellationToken) =>
        this.InnerService.UpdatePrincipalNameAsync(userCredential, principalName, cancellationToken);

    public Task<PrincipalData> RemovePrincipalAsync(UserCredential userCredential, bool force, CancellationToken cancellationToken = default) =>
        this.InnerService.RemovePrincipalAsync(userCredential, force, cancellationToken);

    public Task<MergeResult<PermissionData, PermissionData>> UpdatePermissionsAsync(UserCredential userCredential,
        IEnumerable<ManagedPermission> managedPermissions, CancellationToken cancellationToken = default) =>
        this.InnerService.UpdatePermissionsAsync(userCredential, managedPermissions, cancellationToken);
}

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
        string principalName,
        IEnumerable<ManagedPermission> managedPermissions,
        CancellationToken cancellationToken)
    {
        var principal = await principalDomainService.GetOrCreateAsync(principalName, cancellationToken);

        var result = await this.UpdatePermissionsAsync(principal, [], managedPermissions, cancellationToken);

        return new PrincipalData<TPrincipal, TPermission, TPermissionRestriction>(principal,
            [.. result.AddingItems.Cast<PermissionData<TPermission, TPermissionRestriction>>()]);
    }

    public async Task<PrincipalData> UpdatePrincipalNameAsync(
        UserCredential userCredential,
        string principalName,
        CancellationToken cancellationToken)
    {
        var principal = await principalUserSource.GetUserAsync(userCredential, cancellationToken);

        principalVisualIdentityInfo.Name.Setter(principal, principalName);

        await genericRepository.SaveAsync(principal, cancellationToken);

        return await this.ToPrincipalData(principal, cancellationToken);
    }

    public async Task<PrincipalData> RemovePrincipalAsync(UserCredential userCredential, bool force, CancellationToken cancellationToken)
    {
        var principal = await principalUserSource.GetUserAsync(userCredential, cancellationToken);

        var principalData = await this.ToPrincipalData(principal, cancellationToken);

        await principalDomainService.RemoveAsync(principal, force, cancellationToken);

        return principalData;
    }

    private async Task<PrincipalData<TPrincipal, TPermission, TPermissionRestriction>> ToPrincipalData(TPrincipal dbPrincipal,
        CancellationToken cancellationToken)
    {
        var permissionsData = await permissionLoader
            .LoadAsync(dbPrincipal)
            .Select(permissionRestrictionLoader.ToPermissionData)
            .ToImmutableArrayAsync(cancellationToken);

        return new PrincipalData<TPrincipal, TPermission, TPermissionRestriction>(dbPrincipal, permissionsData);
    }

    public async Task<MergeResult<PermissionData, PermissionData>> UpdatePermissionsAsync(
        UserCredential userCredential,
        IEnumerable<ManagedPermission> managedPermissions,
        CancellationToken cancellationToken)
    {
        var dbPrincipal = await principalUserSource.GetUserAsync(userCredential, cancellationToken);

        var dbPermissions = await permissionLoader.LoadAsync(dbPrincipal).ToArrayAsync(cancellationToken);

        return await this.UpdatePermissionsAsync(dbPrincipal, dbPermissions, managedPermissions, cancellationToken);
    }

    private async Task<MergeResult<PermissionData, PermissionData>> UpdatePermissionsAsync(
        TPrincipal dbPrincipal,
        TPermission[] dbPermissions,
        IEnumerable<ManagedPermission> managedPermissions,
        CancellationToken cancellationToken)
    {
        var permissionMergeResult = dbPermissions.GetMergeResult(managedPermissions, permissionIdentityManager.GetIdentity,
            p => p.Identity.IsDefault ? new object() : permissionIdentityManager.Converter.Convert(p.Identity));

        var newPermissions = await permissionMergeResult
            .AddingItems
            .ToAsyncEnumerable()
            .Select(async (managedPermission, ct) => await permissionManagementService.CreatePermissionAsync(dbPrincipal, managedPermission, ct))
            .ToArrayAsync(cancellationToken);

        var updatedPermissions = await permissionMergeResult
            .CombineItems
            .ToAsyncEnumerable()
            .Select(async (permissionPair, ct) => await permissionManagementService.UpdatePermission(permissionPair.Item1, permissionPair.Item2, ct))
            .ToArrayAsync(cancellationToken);

        var removingPermissions = await permissionMergeResult
            .RemovingItems
            .ToAsyncEnumerable()
            .Select(async (oldDbPermission, ct) =>
            {
                var result = await permissionRestrictionLoader.ToPermissionData(oldDbPermission, ct);

                foreach (var dbRestriction in result.Restrictions)
                {
                    await genericRepository.RemoveAsync(dbRestriction, ct);
                }

                await genericRepository.RemoveAsync(oldDbPermission, ct);

                return result;
            }).ToArrayAsync(cancellationToken);

        await principalValidator.ValidateAsync(
            new PrincipalData<TPrincipal, TPermission, TPermissionRestriction>(
                dbPrincipal,
                [..updatedPermissions.Select(pair => pair.PermissonData), .. newPermissions]),
            cancellationToken);

        return new MergeResult<PermissionData, PermissionData>(
            newPermissions,
            updatedPermissions.Where(pair => pair.Updated).Select(PermissionData (pair) => pair.PermissonData).Select(v => (v, v)),
            removingPermissions);
    }
}