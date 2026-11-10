namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Creates sources of available client rules for a given user.</summary>
public interface IAvailableClientSecurityRuleSourceFactory
{
    /// <summary>Creates a source of available client rules for the given user.</summary>
    IAvailableClientSecurityRuleSource Create(SecurityRuleCredential securityRuleCredential);
}