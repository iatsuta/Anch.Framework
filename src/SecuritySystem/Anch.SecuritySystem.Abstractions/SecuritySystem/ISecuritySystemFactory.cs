// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Creates an <see cref="ISecuritySystem"/> that checks access for a specific user instead of the current one.</summary>
public interface ISecuritySystemFactory
{
    /// <summary>Creates the security system.</summary>
    /// <param name="securityRuleCredential">Whose permissions are checked.</param>
    ISecuritySystem Create(SecurityRuleCredential securityRuleCredential);
}
