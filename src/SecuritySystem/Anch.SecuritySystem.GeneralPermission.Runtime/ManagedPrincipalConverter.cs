using Anch.Core;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.GeneralPermission;

public class ManagedPrincipalConverter<TPrincipal, TPermission, TPermissionRestriction>(
    IManagedPrincipalHeaderConverter<TPrincipal> headerConverter,
    IPermissionLoader<TPrincipal, TPermission> permissionLoader,
    IPermissionManagementService<TPrincipal, TPermission, TPermissionRestriction> permissionManagementService) : IManagedPrincipalConverter<TPrincipal>
    where TPrincipal : class
    where TPermission : class
{
    public async Task<ManagedPrincipal> ToManagedPrincipalAsync(TPrincipal dbPrincipal, CancellationToken ct)
    {
        var permissions = await permissionLoader
            .LoadAsync(dbPrincipal)
            .Select(async (v, lct) => await permissionManagementService.ToManagedPermissionAsync(v, lct))
            .ToImmutableArrayAsync(ct);

        return new ManagedPrincipal(headerConverter.Convert(dbPrincipal), permissions);
    }
}