using System.Collections.Immutable;

using Anch.HierarchicalExpand;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public record SecurityRoleInfo(TypedSecurityIdentity Identity)
{
    public HierarchicalExpandType? CustomExpandType { get; init; } = null;

    public SecurityPathRestriction Restriction { get; init; } = SecurityPathRestriction.Default;

    public ImmutableArray<SecurityOperation> Operations { get; init; } = [];

    public ImmutableArray<SecurityRole> Children { get; init; } = [];

    public string? Description { get; init; }

    public bool IsVirtual { get; init; }
}
