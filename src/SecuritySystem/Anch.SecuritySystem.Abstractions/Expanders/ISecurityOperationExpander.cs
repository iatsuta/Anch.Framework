namespace Anch.SecuritySystem.Expanders;

/// <summary>Replaces an operation with the roles that include it.</summary>
public interface ISecurityOperationExpander
{
    /// <summary>Expands the operation rule.</summary>
    /// <param name="securityRule">The operation rule.</param>
    DomainSecurityRule.NonExpandedRolesSecurityRule Expand(DomainSecurityRule.OperationSecurityRule securityRule);
}
