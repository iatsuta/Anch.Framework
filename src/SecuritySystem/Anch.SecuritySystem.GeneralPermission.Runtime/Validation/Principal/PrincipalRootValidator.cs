using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.GeneralPermission.Validation.Permission;
using Anch.SecuritySystem.Validation;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.GeneralPermission.Validation.Principal;

public class PrincipalRootValidator<TPrincipal, TPermission, TPermissionRestriction>(
    [FromKeyedServices(ISecurityValidator.ElementKey)] IEnumerable<IPrincipalValidator<TPrincipal, TPermission, TPermissionRestriction>> validators,
    IPermissionValidator<TPermission, TPermissionRestriction> permissionRootValidator)
    : IPrincipalValidator<TPrincipal, TPermission, TPermissionRestriction>
{
    public async Task ValidateAsync(PrincipalData<TPrincipal, TPermission, TPermissionRestriction> principalData, CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            await validator.ValidateAsync(principalData, cancellationToken);
        }

        foreach (var permission in principalData.PermissionDataList)
        {
            await permissionRootValidator.ValidateAsync(permission, cancellationToken);
        }
    }
}