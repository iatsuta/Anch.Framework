namespace Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;

/// <summary>The configured security administrator rule.</summary>
/// <param name="SecurityRole">The rule that defines who is a security administrator.</param>
public record SecurityAdministratorRuleInfo(DomainSecurityRule.RoleBaseSecurityRule SecurityRole);
