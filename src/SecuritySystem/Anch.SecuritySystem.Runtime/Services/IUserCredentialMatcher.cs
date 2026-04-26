namespace Anch.SecuritySystem.Services;

public interface IUserCredentialMatcher<in TUser>
{
	bool IsMatch(UserCredential userCredential, TUser user);
}