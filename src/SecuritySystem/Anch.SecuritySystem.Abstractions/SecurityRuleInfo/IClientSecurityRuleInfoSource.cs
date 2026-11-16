namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Supplies the registered client rules.</summary>
public interface IClientSecurityRuleInfoSource
{
    /// <summary>Key under which the individual sources that make up the composite are registered.</summary>
    public const string ElementKey = "Element";

    /// <summary>Returns the client rules.</summary>
    IEnumerable<ClientSecurityRuleInfo> GetInfos();
}
