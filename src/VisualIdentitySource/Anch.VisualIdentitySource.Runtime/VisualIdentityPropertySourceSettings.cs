using System.Collections.Immutable;

namespace Anch.VisualIdentitySource;

/// <summary>The convention for locating name properties.</summary>
/// <param name="PropertyNameList">Candidate property names, in order of priority.</param>
public record VisualIdentityPropertySourceSettings(ImmutableArray<string> PropertyNameList)
{
    /// <summary>The candidates are <c>Login</c>, <c>Name</c> and <c>Code</c>, in this order.</summary>
    public static VisualIdentityPropertySourceSettings Default { get; } = new(["Login", "Name", "Code"]);
}