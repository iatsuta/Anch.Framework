namespace Anch.SecuritySystem.Services;

public record ImpersonateState : IImpersonateState
{
    public UserCredential? CustomUserCredential { get; set; }
}