using System.Collections.Immutable;

using Anch.HierarchicalExpand;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Registration data of a role: identifier, restrictions, included operations, child roles.</summary>
/// <param name="Identity">Stable identifier of the role (for example stored with permissions).</param>
public record SecurityRoleInfo(TypedSecurityIdentity Identity)
{
    /// <summary>How security contexts are expanded for this role; <c>null</c> for the default.</summary>
    public HierarchicalExpandType? CustomExpandType { get; init; } = null;

    /// <summary>Restrictions on the security contexts of the role.</summary>
    public SecurityPathRestriction Restriction { get; init; } = SecurityPathRestriction.Default;

    /// <summary>Operations that this role includes.</summary>
    public ImmutableArray<SecurityOperation> Operations { get; init; } = [];

    /// <summary>Roles whose rights this role also has.</summary>
    public ImmutableArray<SecurityRole> Children { get; init; } = [];

    /// <summary>Human-readable description.</summary>
    public string? Description { get; init; }

    /// <summary>When <c>true</c>, the role is not stored with permissions but computed (see VirtualPermission).</summary>
    public bool IsVirtual { get; init; }
}
