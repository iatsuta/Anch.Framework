using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database;

/// <summary>Settings of the test database: the connection string, how the reference database is initialized and whether a failed database is removed.</summary>
public record TestDatabaseSettings
{
    public required TestConnectionString RawConnectionString { get; init; }

    public DatabaseInitMode InitMode { get; init; } = DatabaseInitMode.RebuildSnapshot;

    public bool RemoveDatabaseOnFailure { get; init; } = true;
}