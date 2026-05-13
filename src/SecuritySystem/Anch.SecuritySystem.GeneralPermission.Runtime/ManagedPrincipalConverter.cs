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
    public async Task<ManagedPrincipal> ToManagedPrincipalAsync(TPrincipal dbPrincipal, CancellationToken cancellationToken)
    {
        var permissions = await permissionLoader
            .LoadAsync(dbPrincipal)
            .Select(async (v, ct) => await permissionManagementService.ToManagedPermissionAsync(v, ct))
            .ToImmutableArrayAsync(cancellationToken);

        return new ManagedPrincipal(headerConverter.Convert(dbPrincipal), permissions);
    }
}