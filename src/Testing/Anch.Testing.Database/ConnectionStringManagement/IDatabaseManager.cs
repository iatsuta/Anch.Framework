namespace Anch.Testing.Database.ConnectionStringManagement;

public interface IDatabaseManager
{
    ValueTask<bool> Exists(TestConnectionString connectionString, CancellationToken ct);

    ValueTask Remove(TestConnectionString connectionString, CancellationToken ct);

    ValueTask Copy(TestConnectionString from, TestConnectionString to, bool force, CancellationToken ct);

    ValueTask Move(TestConnectionString from, TestConnectionString to, bool force, CancellationToken ct);
}