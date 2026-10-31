namespace Anch.SecuritySystem.UserSource;

/// <summary>Creates the error for a user that was not found.</summary>
public interface IMissedUserErrorSource
{
    Exception GetNotFoundException(Type userType, UserCredential userCredential);
}