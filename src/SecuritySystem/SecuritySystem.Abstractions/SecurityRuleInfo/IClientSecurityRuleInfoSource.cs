namespace SecuritySystem.SecurityRuleInfo;

public interface IClientSecurityRuleInfoSource
{
    public const string ElementKey = "Element";

    IEnumerable<ClientSecurityRuleInfo> GetInfos();
}
