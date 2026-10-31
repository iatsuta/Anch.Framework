using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

/// <summary>Allows parallel test execution unless a registered constraint forbids it.</summary>
public class ParallelizationSettings([FromKeyedServices] IEnumerable<AllowParallelizationConstraint> constraints) : IParallelizationSettings
{
    public bool AllowParallelization { get; } = constraints.All(c => c.Value);
}