using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database;

public record TestDatabaseSettings
{
    public required TestConnectionString MainConnectionString { get; init; }

    public DatabaseInitMode InitMode { get; init; } = DatabaseInitMode.RebuildSnapshot;

    public bool RemoveDatabaseOnFailure { get; init; } = true;
}