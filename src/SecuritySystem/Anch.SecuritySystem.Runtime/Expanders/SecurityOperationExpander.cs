using System.Collections.Concurrent;

namespace Anch.SecuritySystem.Expanders;

/// <summary>Replaces an operation with the roles that include it.</summary>
public class SecurityOperationExpander(ISecurityRoleSource securityRoleSource, ISecurityOperationInfoSource securityOperationInfoSource)
    : ISecurityOperationExpander
{
    private readonly ConcurrentDictionary<DomainSecurityRule.OperationSecurityRule, DomainSecurityRule.NonExpandedRolesSecurityRule> cache = [];

    public DomainSecurityRule.NonExpandedRolesSecurityRule Expand(DomainSecurityRule.OperationSecurityRule baseSecurityRule)
    {
        return baseSecurityRule.WithDefaultCredential(securityRule => this.cache.GetOrAdd(securityRule, _ =>
        {
            var securityRoles = securityRoleSource.SecurityRoles
                .Where(sr => sr.Information.Operations.Contains(securityRule.SecurityOperation))
                .ToArray();

            if (securityRoles.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(securityRule), $"No security roles found for operation \"{securityRule.SecurityOperation}\"");
            }

            return securityRoles.ToSecurityRule(
                securityRule.CustomExpandType
                ?? securityOperationInfoSource.GetSecurityOperationInfo(securityRule.SecurityOperation).CustomExpandType,
                securityRule.CustomRestriction);
        }));
    }
}