namespace Anch.SecuritySystem.UserSource;

/// <summary>Supplies a user when none is found, for example by creating it on first access.</summary>
/// <typeparam name="TUser">The user entity type.</typeparam>
public interface IMissedUserService<out TUser>
{
    /// <summary>Returns the user for the credential.</summary>
    /// <param name="userCredential">The user that was not found.</param>
    TUser GetUser(UserCredential userCredential);

    /// <summary>Returns a view that yields the simple <see cref="User"/> record.</summary>
    IMissedUserService<User> ToSimple();
}