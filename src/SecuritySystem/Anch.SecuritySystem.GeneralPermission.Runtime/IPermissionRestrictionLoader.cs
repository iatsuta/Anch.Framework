using Anch.Core;
using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.GeneralPermission;

public interface IPermissionRestrictionLoader<TPermission, TPermissionRestriction>
{
    IAsyncEnumerable<TPermissionRestriction> LoadAsync(TPermission permission);

    async ValueTask<PermissionData<TPermission, TPermissionRestriction>> ToPermissionData(TPermission dbPermission, CancellationToken cancellationToken)
    {
        var dbRestrictions = await this.LoadAsync(dbPermission).ToImmutableArrayAsync(cancellationToken);

        return new PermissionData<TPermission, TPermissionRestriction>(dbPermission, dbRestrictions);
    }
}