using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.GeneralPermission.Validation.PermissionRestriction;
using Anch.SecuritySystem.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.GeneralPermission.Validation.Permission;

public class PermissionRootValidator<TPermission, TPermissionRestriction>(
    [FromKeyedServices(ISecurityValidator.ElementKey)]IEnumerable<IPermissionValidator<TPermission, TPermissionRestriction>> validators,
    IPermissionRestrictionValidator<TPermissionRestriction> permissionRestrictionRootValidator)
    : IPermissionValidator<TPermission, TPermissionRestriction>
{
    public async Task ValidateAsync(PermissionData<TPermission, TPermissionRestriction> permissionData, CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            await validator.ValidateAsync(permissionData, cancellationToken);
        }

        foreach (var restriction in permissionData.Restrictions)
        {
            await permissionRestrictionRootValidator.ValidateAsync(restriction, cancellationToken);
        }
    }
}