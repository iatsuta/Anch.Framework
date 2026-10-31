namespace Anch.SecuritySystem.Services;

/// <summary>Checks whether a user entity matches a user credential.</summary>
public interface IUserCredentialMatcher<in TUser>
{
    bool IsMatch(UserCredential userCredential, TUser user);
}