namespace Anch.Testing.Database;

/// <summary>
/// Defines the database initialization strategy for integration tests.
/// </summary>
public enum DatabaseInitMode
{
    /// <summary>
    /// Forces snapshot rebuild.
    ///
    /// Existing snapshots are ignored and overwritten.
    /// Full cycle: schema → empty snapshot → data → final snapshot.
    /// </summary>
    RebuildSnapshot,

    /// <summary>
    /// Reuses existing snapshots.
    ///
    /// If snapshots exist, they are reused.
    /// If not, they are created.
    /// </summary>
    ReuseSnapshot,

    /// <summary>
    /// Uses an external database.
    ///
    /// No initialization is performed.
    /// </summary>
    External
}