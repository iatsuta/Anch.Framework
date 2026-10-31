namespace Anch.SecuritySystem.UserSource;

/// <summary>Creates the error for a user that was not found.</summary>
public class MissedUserErrorSource : IMissedUserErrorSource
{
    public Exception GetNotFoundException(Type userType, UserCredential userCredential)
    {
        return new UserSourceException($"{userType.Name} \"{userCredential}\" not found");
    }
}