namespace Anch.SecuritySystem.Services;

/// <summary>Holds the user that is currently impersonated.</summary>
public interface IImpersonateState
{
    UserCredential? CustomUserCredential { get; }
}