using System.Collections.Immutable;

namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>Untyped description of a stored principal, passed to listeners.</summary>
public abstract record PrincipalData
{
    /// <summary>The principal entity type.</summary>
    public abstract Type PrincipalType { get; }

    /// <summary>The permission entity type.</summary>
    public abstract Type PermissionType { get; }

    /// <summary>The permission restriction entity type.</summary>
    public abstract Type PermissionRestrictionType { get; }
}

/// <summary>A stored principal entity with its permissions.</summary>
/// <param name="Principal">The principal entity.</param>
/// <param name="PermissionDataList">The permissions of the principal.</param>
/// <typeparam name="TPrincipal">The principal entity type.</typeparam>
/// <typeparam name="TPermission">The permission entity type.</typeparam>
/// <typeparam name="TPermissionRestriction">The permission restriction entity type.</typeparam>
public record PrincipalData<TPrincipal, TPermission, TPermissionRestriction>(
    TPrincipal Principal,
    ImmutableArray<PermissionData<TPermission, TPermissionRestriction>> PermissionDataList) : PrincipalData<TPrincipal>(Principal)
{
    public PrincipalData(TPrincipal principal, IEnumerable<PermissionData<TPermission, TPermissionRestriction>> permissionDataList)
        : this(principal, [.. permissionDataList])
    {
    }

    public override Type PermissionType { get; } = typeof(TPrincipal);

    public override Type PermissionRestrictionType { get; } = typeof(TPrincipal);
}

/// <summary>A stored principal entity.</summary>
/// <param name="Principal">The principal entity.</param>
/// <typeparam name="TPrincipal">The principal entity type.</typeparam>
public abstract record PrincipalData<TPrincipal>(TPrincipal Principal) : PrincipalData
{
    public override Type PrincipalType { get; } = typeof(TPrincipal);
}
