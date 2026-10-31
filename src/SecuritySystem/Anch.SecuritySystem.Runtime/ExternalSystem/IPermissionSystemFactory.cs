namespace Anch.SecuritySystem.ExternalSystem;

/// <summary>Creates a permission system for a user credential.</summary>
public interface IPermissionSystemFactory
{
    IPermissionSystem Create(SecurityRuleCredential securityRuleCredential);
}
