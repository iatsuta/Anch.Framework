using Anch.HierarchicalExpand;

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public record SecurityOperationInfo
{
    public HierarchicalExpandType? CustomExpandType { get; init; } = null;

    public string? Description { get; init; } = null;
}
