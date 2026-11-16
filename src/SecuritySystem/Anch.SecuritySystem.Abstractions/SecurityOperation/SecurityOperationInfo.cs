using Anch.HierarchicalExpand;

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Registration data of an operation.</summary>
public record SecurityOperationInfo
{
    /// <summary>How security contexts are expanded for this operation; <c>null</c> for the default.</summary>
    public HierarchicalExpandType? CustomExpandType { get; init; } = null;

    /// <summary>Human-readable description.</summary>
    public string? Description { get; init; } = null;
}
