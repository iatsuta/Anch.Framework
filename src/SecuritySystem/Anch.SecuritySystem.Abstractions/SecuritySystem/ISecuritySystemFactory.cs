// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public interface ISecuritySystemFactory
{
    ISecuritySystem Create(SecurityRuleCredential securityRuleCredential);
}
