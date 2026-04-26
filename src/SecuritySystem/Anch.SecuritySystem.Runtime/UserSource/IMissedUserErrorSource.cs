namespace Anch.SecuritySystem.UserSource;

public interface IMissedUserErrorSource
{
    Exception GetNotFoundException(Type userType, UserCredential userCredential);
}