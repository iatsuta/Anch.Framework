using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericRepository;

namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Loads the restriction entities of a permission.</summary>
public class PermissionRestrictionLoader<TPermission, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>(
    IQueryableSource queryableSource,
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission> restrictionBindingInfo)
    : IPermissionRestrictionLoader<TPermission, TPermissionRestriction>
    where TPermission : class
    where TPermissionRestriction : class
{
    public IAsyncEnumerable<TPermissionRestriction> LoadAsync(TPermission permission) =>

        queryableSource.GetQueryable<TPermissionRestriction>()
            .Where(restrictionBindingInfo.Permission.Path.Select(p => p == permission))
            .GenericAsAsyncEnumerable();
}