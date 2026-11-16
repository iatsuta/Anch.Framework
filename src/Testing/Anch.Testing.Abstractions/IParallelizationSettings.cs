namespace Anch.Testing;

/// <summary>Settings of parallel test execution.</summary>
public interface IParallelizationSettings
{
    /// <summary>Whether tests may run in parallel.</summary>
    bool AllowParallelization { get; }
}