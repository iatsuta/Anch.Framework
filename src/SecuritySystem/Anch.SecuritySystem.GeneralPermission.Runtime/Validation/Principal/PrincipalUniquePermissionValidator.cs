using Anch.Core;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.Validation;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.GeneralPermission.Validation.Principal;

public class PrincipalUniquePermissionValidator<TPrincipal, TPermission, TPermissionRestriction>(
    IVisualIdentityInfo<TPrincipal> principalVisualIdentityInfo,
    IDisplayPermissionService<TPermission, TPermissionRestriction> displayPermissionService,
    IPermissionEqualityComparer<TPermission, TPermissionRestriction> comparer)
    : IPrincipalValidator<TPrincipal, TPermission, TPermissionRestriction>
{
    public async Task ValidateAsync(PrincipalData<TPrincipal, TPermission, TPermissionRestriction> principalData, CancellationToken cancellationToken)
    {
        var duplicates = await principalData
            .PermissionDataList
            .ToAsyncEnumerable()
            .GroupBy(permission => permission, comparer)
            .Where(g => g.Count() > 1)
            .ToListAsync(cancellationToken);

        if (duplicates.Count > 0)
        {
            var messageBody = duplicates.Join(",", g => $"({displayPermissionService.Format(g.Key)})");

            var message = $"Principal \"{principalVisualIdentityInfo.Name.Getter(principalData.Principal)}\" has duplicate permissions: {messageBody}";

            throw new SecuritySystemValidationException(message);
        }
    }
}