using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Sqlite;

/// <summary>Manages SQLite test databases as files: a copy of a database is a copy of its file.</summary>
public class SqliteDatabaseManager(IDatabaseFilePathExtractor pathExtractor) : IDatabaseManager
{
    public ValueTask CreateEmpty(TestConnectionStringRole connectionStringRole, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var targetPath = pathExtractor.Extract(connectionStringRole);

        var directory = Path.GetDirectoryName(targetPath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> Exists(TestConnectionStringRole connectionStringRole, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var filePath = pathExtractor.Extract(connectionStringRole);

        return new(File.Exists(filePath) && new FileInfo(filePath).Length > 0);
    }

    public ValueTask Remove(TestConnectionStringRole connectionStringRole, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var filePath = pathExtractor.Extract(connectionStringRole);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Copy(TestConnectionStringRole source, TestConnectionStringRole target, CancellationToken ct) => this.CopyMove(source, target, false, ct);

    public ValueTask Move(TestConnectionStringRole source, TestConnectionStringRole target, CancellationToken ct) => this.CopyMove(source, target, true, ct);


    private ValueTask CopyMove(
        TestConnectionStringRole source,
        TestConnectionStringRole target,
        bool move,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var sourcePath = pathExtractor.Extract(source);
        var targetPath = pathExtractor.Extract(target);

        if (File.Exists(sourcePath))
        {
            File.Delete(targetPath);
        }
        else
        {
            throw new FileNotFoundException("Source database file not found.", sourcePath);
        }

        if (move)
        {
            File.Move(sourcePath, targetPath, overwrite: true);
        }
        else
        {
            File.Copy(sourcePath, targetPath, overwrite: true);
        }

        return ValueTask.CompletedTask;
    }
}