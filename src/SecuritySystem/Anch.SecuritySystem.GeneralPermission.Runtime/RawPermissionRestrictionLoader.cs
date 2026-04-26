namespace Anch.SecuritySystem.GeneralPermission;
public class RawPermissionRestrictionLoader<TPermission, TPermissionRestriction>(
    IPermissionRestrictionLoader<TPermission, TPermissionRestriction> permissionRestrictionLoader,
    IPermissionRestrictionRawConverter<TPermissionRestriction> permissionRestrictionRawConverter) : IRawPermissionRestrictionLoader<TPermission>
{
    public async ValueTask<Dictionary<Type, Array>> LoadAsync(TPermission permission, CancellationToken cancellationToken)
    {
        var dbRestrictions = await permissionRestrictionLoader.LoadAsync(permission).ToArrayAsync(cancellationToken);

        return permissionRestrictionRawConverter.Convert(dbRestrictions);
    }
}