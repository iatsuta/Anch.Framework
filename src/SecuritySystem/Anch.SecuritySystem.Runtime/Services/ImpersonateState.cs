namespace Anch.SecuritySystem.Services;

/// <summary>The mutable impersonation state of the current scope.</summary>
public record ImpersonateState : IImpersonateState
{
    public UserCredential? CustomUserCredential { get; set; }
}