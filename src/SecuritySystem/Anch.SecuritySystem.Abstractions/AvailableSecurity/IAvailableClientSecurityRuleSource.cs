namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Lists the client rules that the current user satisfies, for example to decide which buttons a UI shows.</summary>
public interface IAvailableClientSecurityRuleSource
{
    /// <summary>Returns the available client rules.</summary>
    IAsyncEnumerable<DomainSecurityRule.ClientSecurityRule> GetAvailableSecurityRules();
}