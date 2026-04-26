using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericRepository;

namespace Anch.SecuritySystem.GeneralPermission;

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