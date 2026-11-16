namespace Anch.Testing;

/// <summary>A vote on parallel test execution. When any registered constraint is <c>false</c>, tests run one at a time.</summary>
/// <param name="Value">Whether parallel execution is acceptable.</param>
public record AllowParallelizationConstraint(bool Value);