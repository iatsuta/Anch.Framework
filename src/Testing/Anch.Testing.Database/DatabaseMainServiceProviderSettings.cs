namespace Anch.Testing.Database;

/// <summary>Decides whether the main container is returned to the pool: only when tests do not run in parallel.</summary>
public class DatabaseMainServiceProviderSettings(IParallelizationSettings parallelizationSettings, TestDatabaseSettings testDatabaseSettings)
    : IMainServiceProviderSettings
{
    public bool ReturnToPool { get; } = !parallelizationSettings.AllowParallelization || testDatabaseSettings.InitMode == DatabaseInitMode.External;
}