using System.Collections.Immutable;
using System.Linq.Expressions;

namespace Anch.SecuritySystem.ExternalSystem;

public interface IPermissionSource
{
    ValueTask<bool> HasAccessAsync(CancellationToken cancellationToken);

    IAsyncEnumerable<Dictionary<Type, Array>> GetPermissionsAsync(ImmutableArray<Type> securityContextTypes);
}

public interface IPermissionSource<TPermission> : IPermissionSource
{
    IQueryable<TPermission> GetPermissionQuery();

    IAsyncEnumerable<string> GetAccessorsAsync(Expression<Func<TPermission, bool>> permissionFilter);
}
