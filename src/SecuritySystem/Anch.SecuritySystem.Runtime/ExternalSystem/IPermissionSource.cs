using System.Collections.Immutable;
using System.Linq.Expressions;

namespace Anch.SecuritySystem.ExternalSystem;

/// <summary>Reads the permissions of a permission system: checks access and provides queries over the permissions that satisfy a rule.</summary>
public interface IPermissionSource
{
    Task<bool> HasAccessAsync(CancellationToken ct);

    IAsyncEnumerable<Dictionary<Type, Array>> GetPermissionsAsync(ImmutableArray<Type> securityContextTypes);
}

/// <summary>Reads the permissions of a permission system: checks access and provides queries over the permissions that satisfy a rule.</summary>
public interface IPermissionSource<TPermission> : IPermissionSource
{
    IQueryable<TPermission> GetPermissionQuery();

    IAsyncEnumerable<string> GetAccessorsAsync(Expression<Func<TPermission, bool>> permissionFilter);
}
