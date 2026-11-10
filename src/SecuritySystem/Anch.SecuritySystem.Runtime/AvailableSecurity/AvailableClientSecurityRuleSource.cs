using Anch.SecuritySystem.SecurityRuleInfo;

namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Lists the client rules satisfied by the roles that the current user has.</summary>
public class AvailableClientSecurityRuleSource(
    IClientSecurityRuleInfoSource clientSecurityRuleInfoSource,
    ISecuritySystem securitySystem) : IAvailableClientSecurityRuleSource
{
    public IAsyncEnumerable<DomainSecurityRule.ClientSecurityRule> GetAvailableSecurityRules() =>

        clientSecurityRuleInfoSource
            .GetInfos()
            .ToAsyncEnumerable()
            .Where(async (clientSecurityRuleInfo, ct) => await securitySystem.HasAccessAsync(clientSecurityRuleInfo.Implementation, ct))
            .Select(clientSecurityRuleInfo => clientSecurityRuleInfo.Rule);
}