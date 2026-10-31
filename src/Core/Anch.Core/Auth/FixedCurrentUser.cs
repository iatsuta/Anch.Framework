namespace Anch.Core.Auth;

/// <summary>An <see cref="ICurrentUser"/> with a fixed name.</summary>
/// <param name="name">The user name.</param>
public class FixedCurrentUser(string name) : ICurrentUser
{
    /// <inheritdoc/>
    public string Name => name;

    /// <summary>The operating system user of the running process, as <c>Machine\User</c>.</summary>
    public static FixedCurrentUser CurrentMachine { get; } = new(
        $"{Environment.MachineName}\\{Environment.UserName}");
}