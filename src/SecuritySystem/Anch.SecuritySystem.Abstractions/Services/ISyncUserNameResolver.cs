namespace Anch.SecuritySystem.Services;

public interface ISyncUserNameResolver
{
    string GetUserName(UserCredential userCredential);
}