using Microsoft.Extensions.DependencyInjection;

namespace SecuritySystem.SecurityRuleInfo;

public class RootClientSecurityRuleInfoSource(
    [FromKeyedServices(IClientSecurityRuleInfoSource.ElementKey)]
    IEnumerable<IClientSecurityRuleInfoSource> elements)
    : IClientSecurityRuleInfoSource
{
    private readonly ClientSecurityRuleInfo[] infos = elements.SelectMany(el => el.GetInfos()).Distinct().ToArray();

    public IEnumerable<ClientSecurityRuleInfo> GetInfos() => this.infos;
}
