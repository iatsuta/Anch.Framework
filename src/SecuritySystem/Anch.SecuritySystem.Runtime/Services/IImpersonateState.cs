namespace Anch.SecuritySystem.Services;

public interface IImpersonateState
{
    UserCredential? CustomUserCredential { get; }
}