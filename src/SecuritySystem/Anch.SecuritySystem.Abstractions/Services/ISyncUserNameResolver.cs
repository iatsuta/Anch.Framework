namespace Anch.SecuritySystem.Services;

/// <summary>Resolves the name of a user synchronously.</summary>
public interface ISyncUserNameResolver
{
    /// <summary>Returns the user name.</summary>
    /// <param name="userCredential">The user.</param>
    string GetUserName(UserCredential userCredential);
}