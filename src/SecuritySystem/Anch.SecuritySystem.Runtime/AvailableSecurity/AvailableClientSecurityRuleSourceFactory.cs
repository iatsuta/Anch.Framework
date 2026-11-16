using Anch.SecuritySystem.SecurityRuleInfo;

namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Creates a client rule source whose rules are checked with the security system of the given user.</summary>
public class AvailableClientSecurityRuleSourceFactory(
    IClientSecurityRuleInfoSource clientSecurityRuleInfoSource,
    ISecuritySystemFactory securitySystemFactory) : IAvailableClientSecurityRuleSourceFactory
{
    public IAvailableClientSecurityRuleSource Create(SecurityRuleCredential securityRuleCredential)
    {
        return new AvailableClientSecurityRuleSource(clientSecurityRuleInfoSource, securitySystemFactory.Create(securityRuleCredential));
    }
}