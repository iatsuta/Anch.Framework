using System.Collections.Immutable;
using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.GeneralPermission;

public class PermissionManagementService<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>(
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo,
    GeneralPermissionBindingInfo<TPermission, TSecurityRole> generalBindingInfo,
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission> restrictionBindingInfo,
    IPermissionSecurityRoleResolver<TPermission> permissionSecurityRoleResolver,
    IRawPermissionRestrictionLoader<TPermission> rawPermissionRestrictionLoader,
    ISecurityIdentityManager<TPermission> permissionSecurityIdentityManager,
    ISecurityRepository<TSecurityRole> securityRoleRepository,
    IQueryableSource queryableSource,
    ISecurityRoleSource securityRoleSource,
    ISecurityRepository<TSecurityContextType> securityContextTypeRepository,
    ISecurityContextInfoSource securityContextInfoSource,
    ISecurityIdentityManager<TSecurityRole> securityRoleIdentityManager,
    ISecurityIdentityManager<TSecurityContextType> securityContextTypeIdentityManager,
    ISecurityIdentityManager<TPermission> permissionIdentityManager,
    IGenericRepository genericRepository,
    ISecurityRepository<TPermission> permissionRepository)
    : IPermissionManagementService<TPrincipal, TPermission, TPermissionRestriction>

    where TPermission : class, new()
    where TSecurityRole : class
    where TPermissionRestriction : class, new()
    where TSecurityContextType : class
    where TSecurityContextObjectIdent : notnull
{
    public virtual async ValueTask<ManagedPermission> ToManagedPermissionAsync(TPermission dbPermission, CancellationToken cancellationToken) =>
        new()
        {
            Identity = permissionSecurityIdentityManager.GetIdentity(dbPermission),
            IsVirtual = bindingInfo.IsReadonly,
            SecurityRole = permissionSecurityRoleResolver.Resolve(dbPermission),
            Period = bindingInfo.GetSafePeriod(dbPermission),
            Comment = bindingInfo.GetSafeComment(dbPermission),
            DelegatedFrom = bindingInfo.DelegatedFrom?.Getter.Invoke(dbPermission) is { } delegatedFromPermission
                ? permissionIdentityManager.GetIdentity(delegatedFromPermission)
                : SecurityIdentity.Default,
            Restrictions = (await rawPermissionRestrictionLoader.LoadAsync(dbPermission, cancellationToken)).ToImmutableDictionary()
        };

    public virtual async ValueTask<PermissionData<TPermission, TPermissionRestriction>> CreatePermissionAsync(
        TPrincipal dbPrincipal,
        ManagedPermission managedPermission,
        CancellationToken cancellationToken)
    {
        if (managedPermission.IsVirtual || (!managedPermission.Identity.IsDefault && !managedPermission.ForceApplyIdentity))
        {
            throw new SecuritySystemException("wrong permission");
        }

        var securityRole = securityRoleSource.GetSecurityRole(managedPermission.SecurityRole);

        var dbRole = await securityRoleRepository.GetObjectAsync(securityRole.Identity, cancellationToken);

        var newDbPermission = new TPermission();

        if (!managedPermission.Identity.IsDefault)
        {
            permissionIdentityManager.SetIdentity(newDbPermission, managedPermission.Identity);
        }

        bindingInfo.Principal.Setter(newDbPermission, dbPrincipal);
        generalBindingInfo.SecurityRole.Setter(newDbPermission, dbRole);

        bindingInfo.PermissionStartDate?.Setter(newDbPermission, managedPermission.Period.StartDate);
        bindingInfo.PermissionEndDate?.Setter(newDbPermission, managedPermission.Period.EndDate);
        bindingInfo.PermissionComment?.Setter(newDbPermission, managedPermission.Comment);

        if (!managedPermission.DelegatedFrom.IsDefault)
        {
            var delegatedFromAccessors = bindingInfo.DelegatedFrom ?? throw new InvalidOperationException("Delegated Permission Binding not initialized");

            var delegatedFromPermission = await permissionRepository.GetObjectAsync(managedPermission.DelegatedFrom, cancellationToken);

            delegatedFromAccessors.Setter(newDbPermission, delegatedFromPermission);
        }

        await genericRepository.SaveAsync(newDbPermission, cancellationToken);

        var newPermissionRestrictions = await managedPermission.Restrictions.ToAsyncEnumerable().SelectMany(async (restrictionGroup, ct) =>
        {
            var securityContextTypeIdentity = securityContextInfoSource.GetSecurityContextInfo(restrictionGroup.Key).Identity;

            var dbSecurityContextType = await securityContextTypeRepository.GetObjectAsync(securityContextTypeIdentity, ct);

            return restrictionGroup.Value.Cast<TSecurityContextObjectIdent>().Select(securityContextId => new { dbSecurityContextType, securityContextId });
        }).Select(async (pair, ct) =>
        {
            var newDbPermissionRestriction = new TPermissionRestriction();

            restrictionBindingInfo.Permission.Setter(newDbPermissionRestriction, newDbPermission);
            restrictionBindingInfo.SecurityContextObjectId.Setter(newDbPermissionRestriction, pair.securityContextId);
            restrictionBindingInfo.SecurityContextType.Setter(newDbPermissionRestriction, pair.dbSecurityContextType);

            await genericRepository.SaveAsync(newDbPermissionRestriction, ct);

            return newDbPermissionRestriction;
        }).ToImmutableArrayAsync(cancellationToken);

        return new PermissionData<TPermission, TPermissionRestriction>(newDbPermission, newPermissionRestrictions);
    }

    public virtual async ValueTask<(PermissionData<TPermission, TPermissionRestriction> PermissonData, bool Updated)> UpdatePermission(
        TPermission dbPermission,
        ManagedPermission managedPermission,
        CancellationToken cancellationToken)
    {
        if (managedPermission.IsVirtual || managedPermission.Identity.IsDefault)
        {
            throw new InvalidOperationException("wrong permission");
        }

        if (!managedPermission.DelegatedFrom.IsDefault)
        {
            var delegatedFromAccessors = bindingInfo.DelegatedFrom ?? throw new InvalidOperationException("Delegated Permission Binding not initialized");

            var delegatedFromPermission = await permissionRepository.GetObjectAsync(managedPermission.DelegatedFrom, cancellationToken);

            if (delegatedFromPermission != delegatedFromAccessors.Getter(dbPermission))
            {
                throw new SecuritySystemException("Delegated source can't be changed");
            }
        }

        var securityRole = generalBindingInfo
            .SecurityRole
            .Getter(dbPermission)
            .Pipe(securityRoleIdentityManager.GetIdentity)
            .Pipe(securityRoleSource.GetSecurityRole);

        if (securityRole != managedPermission.SecurityRole)
        {
            throw new SecuritySystemException("Permission role can't be changed");
        }

        var dbRestrictions = await queryableSource.GetQueryable<TPermissionRestriction>()
            .Where(restrictionBindingInfo.Permission.Path.Select(p => p == dbPermission))
            .GenericToListAsync(cancellationToken);

        var restrictionMergeResult = EnumerableExtensions.GetMergeResult<TPermissionRestriction, (TypedSecurityIdentity Key, TSecurityContextObjectIdent securityContextId), (TypedSecurityIdentity, TSecurityContextObjectIdent)>(dbRestrictions, managedPermission.Restrictions
                .ChangeKey(t => securityContextInfoSource.GetSecurityContextInfo(t).Identity)
                .SelectMany(pair => pair.Value.Cast<TSecurityContextObjectIdent>().Select(securityContextId => (pair.Key, securityContextId))),
            pr => (
                securityContextTypeIdentityManager.GetIdentity(restrictionBindingInfo.SecurityContextType.Getter(pr)),
                restrictionBindingInfo.SecurityContextObjectId.Getter(pr)),
            pair => pair);

        if (restrictionMergeResult.IsEmpty
            && (bindingInfo.PermissionComment == null || bindingInfo.PermissionComment.Getter(dbPermission) == managedPermission.Comment)
            && (bindingInfo.PermissionStartDate == null || bindingInfo.PermissionStartDate.Getter(dbPermission) == managedPermission.Period.StartDate)
            && (bindingInfo.PermissionEndDate == null || bindingInfo.PermissionEndDate.Getter(dbPermission) == managedPermission.Period.EndDate))
        {
            var permissionData = new PermissionData<TPermission, TPermissionRestriction>(
                dbPermission,
                restrictionMergeResult.CombineItems.Select(v => v.Item1));

            return (permissionData, false);
        }
        else
        {
            bindingInfo.PermissionComment?.Setter.Invoke(dbPermission, managedPermission.Comment);
            bindingInfo.PermissionStartDate?.Setter.Invoke(dbPermission, managedPermission.Period.StartDate);
            bindingInfo.PermissionEndDate?.Setter.Invoke(dbPermission, managedPermission.Period.EndDate);

            var newPermissionRestrictions = await restrictionMergeResult.AddingItems.ToAsyncEnumerable().Select(async (restriction, ct) =>
            {
                var dbSecurityContextType =
                    await securityContextTypeRepository.GetObjectAsync(restriction.Key, ct);

                var newPermissionRestriction = new TPermissionRestriction();

                restrictionBindingInfo.Permission.Setter(newPermissionRestriction, dbPermission);
                restrictionBindingInfo.SecurityContextObjectId.Setter(newPermissionRestriction, restriction.securityContextId);
                restrictionBindingInfo.SecurityContextType.Setter(newPermissionRestriction, dbSecurityContextType);

                await genericRepository.SaveAsync(newPermissionRestriction, ct);

                return newPermissionRestriction;
            }).ToArrayAsync(cancellationToken);

            foreach (var dbRestriction in restrictionMergeResult.RemovingItems)
            {
                await genericRepository.RemoveAsync(dbRestriction, cancellationToken);
            }

            var permissionData = new PermissionData<TPermission, TPermissionRestriction>(dbPermission,
                [.. restrictionMergeResult.CombineItems.Select(v => v.Item1), .. newPermissionRestrictions]);

            await genericRepository.SaveAsync(dbPermission, cancellationToken);

            return (permissionData, true);
        }
    }
}