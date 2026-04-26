using Anch.Core;
using Anch.SecuritySystem.Validation;

namespace Anch.SecuritySystem.VirtualPermission;

public class VirtualPermissionBindingInfoValidator(ISecurityRoleSource securityRoleSource) : IVirtualPermissionBindingInfoValidator
{
    public void Validate(VirtualPermissionBindingInfo virtualBindingInfo)
    {
        foreach (var itemBindingInfo in virtualBindingInfo.BaseItems)
        {
            this.Validate(virtualBindingInfo, itemBindingInfo);
        }
    }

    private void Validate(VirtualPermissionBindingInfo virtualBindingInfo, VirtualPermissionSecurityRoleItemBindingInfo itemBindingInfo)
    {
        var securityContextRestrictions = securityRoleSource
            .GetSecurityRole(itemBindingInfo.SecurityRole)
            .Information
            .Restriction
            .SecurityContextRestrictions;

        if (securityContextRestrictions != null)
        {
            var bindingContextTypes = virtualBindingInfo.SecurityContextTypes;

            var invalidTypes = bindingContextTypes.Except(securityContextRestrictions.Select(r => r.SecurityContextType)).ToList();

            if (invalidTypes.Any())
            {
                throw new SecuritySystemValidationException($"Invalid restriction types: {invalidTypes.Join(", ", t => t.Name)}");
            }

            var missedTypes = securityContextRestrictions
                .Where(r => r.Required)
                .Select(r => r.SecurityContextType)
                .Except(bindingContextTypes)
                .ToList();

            if (missedTypes.Any())
            {
                throw new SecuritySystemValidationException($"Missed required restriction types: {missedTypes.Join(", ", t => t.Name)}");
            }
        }
    }
}