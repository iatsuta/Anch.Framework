using Anch.SecuritySystem.Validation;

namespace Anch.SecuritySystem.GeneralPermission.Validation.PermissionRestriction;

public class AllowedTypePermissionRestrictionValidator<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission>(
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission> restrictionBindingInfo,
    IPermissionSecurityRoleResolver<TPermission> permissionSecurityRoleResolver,
    IPermissionRestrictionSecurityContextTypeResolver<TPermissionRestriction> permissionRestrictionSecurityContextTypeResolver)
    : IPermissionRestrictionValidator<TPermissionRestriction>
    where TSecurityContextObjectIdent : notnull
{
    public ValueTask ValidateAsync(TPermissionRestriction permissionRestriction, CancellationToken ct)
    {
        var permission = restrictionBindingInfo.Permission.Getter(permissionRestriction);

        var securityContextType = permissionRestrictionSecurityContextTypeResolver.Resolve(permissionRestriction);

        var securityRole = permissionSecurityRoleResolver.Resolve(permission);

        var allowedSecurityContexts = securityRole.Information.Restriction.SecurityContextTypes;

        var allowed = allowedSecurityContexts is null || allowedSecurityContexts.Contains(securityContextType);

        if (allowed)
        {
            return ValueTask.CompletedTask;
        }
        else
        {
            throw new SecuritySystemValidationException($"Invalid SecurityContextType: {securityContextType.Name}");
        }
    }
}