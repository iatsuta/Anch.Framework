namespace Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;

/// <summary>Built-in rules of the application itself.</summary>
public static class ApplicationSecurityRule
{
    /// <summary>The rule that defines a security administrator; its content is set with <c>SetSecurityAdministratorRule</c>.</summary>
    public static DomainSecurityRule.RoleFactorySecurityRule SecurityAdministrator { get; } = new(typeof(SecurityAdministratorRuleFactory));
}
