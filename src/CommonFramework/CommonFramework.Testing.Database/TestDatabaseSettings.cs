using CommonFramework.Testing.Database.ConnectionStringManagement;

namespace CommonFramework.Testing.Database;

public record TestDatabaseSettings
{
    public required TestDatabaseConnectionString DefaultConnectionString { get; init; }

    public DatabaseInitMode InitMode { get; init; } = DatabaseInitMode.RebuildSnapshot;

    public bool RemoveDatabaseOnFailure { get; init; } = true;
}