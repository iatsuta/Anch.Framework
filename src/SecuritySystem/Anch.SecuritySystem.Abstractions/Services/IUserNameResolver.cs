namespace Anch.SecuritySystem.Services;

/// <summary>Resolves the name of a user.</summary>
public interface IUserNameResolver
{
    /// <summary>Returns the user name.</summary>
    /// <param name="userCredential">The user.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<string> GetUserNameAsync(UserCredential userCredential, CancellationToken ct);
}

/// <summary>Resolves the name of the user a credential denotes, using the user entity type.</summary>
/// <typeparam name="TUser">The user entity type.</typeparam>
public interface IUserNameResolver<out TUser>
{
    /// <summary>Returns the user name.</summary>
    /// <param name="credential">Whose name to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The name, or <c>null</c> when the credential does not denote a single user.</returns>
    Task<string?> GetUserNameAsync(SecurityRuleCredential credential, CancellationToken ct);
}