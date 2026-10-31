using Anch.SecuritySystem.Validation;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.GeneralPermission.Validation.PermissionRestriction;

/// <summary>Runs all restriction validators.</summary>
public class PermissionRestrictionRootValidator<TPermissionRestriction>(
    [FromKeyedServices(ISecurityValidator.ElementKey)]
    IEnumerable<IPermissionRestrictionValidator<TPermissionRestriction>> validators) : IPermissionRestrictionValidator<TPermissionRestriction>
{
    public async ValueTask ValidateAsync(TPermissionRestriction permissionRestriction, CancellationToken ct)
    {
        foreach (var validator in validators)
        {
            await validator.ValidateAsync(permissionRestriction, ct);
        }
    }
}