namespace Anch.Testing.Database.ConnectionStringManagement;

/// <summary>Creates, copies and removes test databases of a particular database engine.</summary>
public interface IDatabaseManager
{
    ValueTask CreateEmpty(TestConnectionStringRole connectionStringRole, CancellationToken ct);

    ValueTask<bool> Exists(TestConnectionStringRole connectionStringRole, CancellationToken ct);

    ValueTask Remove(TestConnectionStringRole connectionStringRole, CancellationToken ct);

    ValueTask Copy(TestConnectionStringRole source, TestConnectionStringRole target, CancellationToken ct);

    ValueTask Move(TestConnectionStringRole source, TestConnectionStringRole target, CancellationToken ct);
}