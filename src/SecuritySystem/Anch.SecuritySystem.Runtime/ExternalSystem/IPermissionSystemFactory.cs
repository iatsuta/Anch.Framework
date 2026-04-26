namespace Anch.SecuritySystem.ExternalSystem;

public interface IPermissionSystemFactory
{
    IPermissionSystem Create(SecurityRuleCredential securityRuleCredential);
}
