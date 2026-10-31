using System.Collections.Immutable;

namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>Untyped description of a stored permission, passed to listeners.</summary>
public abstract record PermissionData
{
    /// <summary>The permission entity type.</summary>
    public abstract Type PermissionType { get; }
}

/// <summary>A stored permission entity.</summary>
/// <param name="Permission">The permission entity.</param>
/// <typeparam name="TPermission">The permission entity type.</typeparam>
public abstract record PermissionData<TPermission>(TPermission Permission) : PermissionData
{
    public override Type PermissionType { get; } = typeof(TPermission);
}

/// <summary>A stored permission entity with its restriction entities.</summary>
/// <param name="Permission">The permission entity.</param>
/// <param name="Restrictions">The restriction entities of the permission.</param>
/// <typeparam name="TPermission">The permission entity type.</typeparam>
/// <typeparam name="TPermissionRestriction">The permission restriction entity type.</typeparam>
public record PermissionData<TPermission, TPermissionRestriction>(TPermission Permission, ImmutableArray<TPermissionRestriction> Restrictions)
    : PermissionData<TPermission>(Permission)
{
    public PermissionData(TPermission permission, IEnumerable<TPermissionRestriction> restrictions)
        : this(permission, [.. restrictions])
    {
    }
}