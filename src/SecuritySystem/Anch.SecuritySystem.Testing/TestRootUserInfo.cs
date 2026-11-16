namespace Anch.SecuritySystem.Testing;

/// <summary>The user that tests run as by default.</summary>
/// <param name="Name">The user name.</param>
public record TestRootUserInfo(string Name)
{
    /// <summary>A user named <c>TestRootUser</c>.</summary>
    public static TestRootUserInfo Default { get; } = new("TestRootUser");
}