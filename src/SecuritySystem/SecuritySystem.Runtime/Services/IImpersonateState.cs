using SecuritySystem.Credential;

namespace SecuritySystem.Services;

public interface IImpersonateState
{
    UserCredential? CustomUserCredential { get; }
}