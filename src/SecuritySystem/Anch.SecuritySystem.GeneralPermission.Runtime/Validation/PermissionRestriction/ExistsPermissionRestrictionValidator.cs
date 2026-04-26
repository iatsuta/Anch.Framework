using Anch.SecuritySystem.ExternalSystem.SecurityContextStorage;
using Anch.SecuritySystem.Validation;

namespace Anch.SecuritySystem.GeneralPermission.Validation.PermissionRestriction;

public class ExistsPermissionRestrictionValidator<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>(
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent> restrictionBindingInfo,
    ISecurityContextStorage securityContextStorage,
    IPermissionRestrictionSecurityContextTypeResolver<TPermissionRestriction> permissionRestrictionSecurityContextTypeResolver)
    : IPermissionRestrictionValidator<TPermissionRestriction>
    where TSecurityContextObjectIdent : notnull
{
    public async Task ValidateAsync(TPermissionRestriction permissionRestriction, CancellationToken cancellationToken)
    {
        var securityContextObjectId = restrictionBindingInfo.SecurityContextObjectId.Getter(permissionRestriction);

        var securityContextType = permissionRestrictionSecurityContextTypeResolver.Resolve(permissionRestriction);

        var typedSecurityContextStorage = securityContextStorage.GetTyped(securityContextType);

        if (!typedSecurityContextStorage.IsExists(TypedSecurityIdentity.Create(securityContextObjectId)))
        {
            throw new SecuritySystemValidationException($"{securityContextType.Name} with id '{securityContextObjectId}' not exists");
        }
    }
}