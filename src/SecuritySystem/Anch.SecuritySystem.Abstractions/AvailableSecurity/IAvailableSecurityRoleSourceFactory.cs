namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Creates sources of available security roles for a given user.</summary>
public interface IAvailableSecurityRoleSourceFactory
{
    /// <summary>Creates an instance of <see cref="IAvailableSecurityRoleSource"/> for the specified security rule credential.</summary>
    IAvailableSecurityRoleSource Create(SecurityRuleCredential securityRuleCredential);
}