namespace Anch.IdentitySource;

/// <summary>The convention for locating identifier properties.</summary>
/// <param name="PropertyName">The name of the identifier property.</param>
public record IdentityPropertySourceSettings(string PropertyName)
{
    /// <summary>The property is named <c>Id</c>.</summary>
    public static IdentityPropertySourceSettings Default { get; } = new("Id");
}