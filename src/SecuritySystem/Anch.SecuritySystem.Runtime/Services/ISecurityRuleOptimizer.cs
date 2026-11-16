namespace Anch.SecuritySystem.Services;

/// <summary>Simplifies a rule. The implementations are registered as keyed services (<see cref="BasicKey"/>, <see cref="AccessKey"/>, <see cref="DeepKey"/>).</summary>
public interface ISecurityRuleOptimizer
{
    /// <summary>The key of the optimizer that simplifies a rule without expanding it.</summary>
    public const string BasicKey = "Basic";

    /// <summary>The key of the optimizer that expands a rule completely and simplifies it.</summary>
    public const string DeepKey = "Deep";

    /// <summary>The key of the optimizer that prepares a rule for an access check: it expands operations, domain modes, headers and client rules down to non-expanded role rules and keeps those as they are.</summary>
    public const string AccessKey = "Access";

    /// <summary>Returns the simplified rule.</summary>
    /// <param name="securityRule">The rule to simplify.</param>
    DomainSecurityRule Optimize(DomainSecurityRule securityRule);
}
