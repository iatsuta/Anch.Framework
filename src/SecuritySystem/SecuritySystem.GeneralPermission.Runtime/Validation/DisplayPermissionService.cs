using CommonFramework;

using SecuritySystem.ExternalSystem.Management;
using SecuritySystem.ExternalSystem.SecurityContextStorage;

namespace SecuritySystem.GeneralPermission.Validation;

public class DisplayPermissionService<TPermission, TPermissionRestriction>(
    PermissionBindingInfo<TPermission> bindingInfo,
    IPermissionSecurityRoleResolver<TPermission> securityRoleResolver,
    ISecurityContextInfoSource securityContextInfoSource,
    ISecurityContextStorage securityContextStorage,
    IPermissionRestrictionRawConverter<TPermissionRestriction> rawPermissionConverter)
    : IDisplayPermissionService<TPermission, TPermissionRestriction>
{
    public string Format(PermissionData<TPermission, TPermissionRestriction> permissionData)
    {
        return this.GetPermissionVisualParts(permissionData).Join(" | ");
    }

    private IEnumerable<string> GetPermissionVisualParts(PermissionData<TPermission, TPermissionRestriction> permissionData)
    {
        var permission = permissionData.Permission;

        yield return $"Role: {securityRoleResolver.Resolve(permissionData.Permission).Name}";

        if (bindingInfo.PermissionStartDate != null)
        {
            yield return $"StartDate: {bindingInfo.PermissionStartDate.Getter(permission)}";
        }

        if (bindingInfo.PermissionEndDate != null)
        {
            yield return $"EndDate: {bindingInfo.PermissionEndDate.Getter(permission)}";
        }

        foreach (var securityContextTypeGroup in rawPermissionConverter.Convert(permissionData.Restrictions))
        {
            var securityContextInfo = securityContextInfoSource.GetSecurityContextInfo(securityContextTypeGroup.Key);

            var securityContextList = securityContextStorage
                .GetTyped(securityContextInfo.Type)
                .GetSecurityContextsByIdents(securityContextTypeGroup.Value);

            yield return $"{securityContextInfo.Name}: {securityContextList.Select(v => v.Name).Join(", ")}";
        }
    }
}