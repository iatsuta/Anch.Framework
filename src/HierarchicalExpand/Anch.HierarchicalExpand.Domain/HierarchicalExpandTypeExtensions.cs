namespace Anch.HierarchicalExpand;

/// <summary>Extensions for <see cref="HierarchicalExpandType"/>.</summary>
public static class HierarchicalExpandTypeExtensions
{
    /// <summary>Swaps <c>Parents</c> and <c>Children</c>; <c>None</c> and <c>All</c> are returned unchanged.</summary>
    /// <param name="hierarchicalExpandType">The value to reverse.</param>
    public static HierarchicalExpandType Reverse(this HierarchicalExpandType hierarchicalExpandType)
    {
        return hierarchicalExpandType switch
        {
            HierarchicalExpandType.Parents => HierarchicalExpandType.Children,
            HierarchicalExpandType.Children => HierarchicalExpandType.Parents,
            _ => hierarchicalExpandType
        };
    }
}