namespace Anch.Testing.Database.ConnectionStringManagement;

public interface IDatabaseManager
{
    ValueTask<bool> Exists(TestDatabaseConnectionString connectionString, CancellationToken ct);

    ValueTask Remove(TestDatabaseConnectionString connectionString, CancellationToken ct);

    ValueTask Copy(TestDatabaseConnectionString from, TestDatabaseConnectionString to, bool force, CancellationToken ct);

    ValueTask Move(TestDatabaseConnectionString from, TestDatabaseConnectionString to, bool force, CancellationToken ct);
}