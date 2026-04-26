namespace Anch.Testing.Database.ConnectionStringManagement;

public class FileDatabaseManager(IDatabaseFilePathExtractor pathExtractor) : IDatabaseManager
{
    public ValueTask<bool> Exists(TestDatabaseConnectionString connectionString, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var filePath = pathExtractor.Extract(connectionString);

        return new(File.Exists(filePath) && new FileInfo(filePath).Length > 0);
    }

    public ValueTask Remove(TestDatabaseConnectionString connectionString, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var filePath = pathExtractor.Extract(connectionString);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Copy(
        TestDatabaseConnectionString from,
        TestDatabaseConnectionString to,
        bool force,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var sourcePath = pathExtractor.Extract(from);
        var destinationPath = pathExtractor.Extract(to);

        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("Source database file not found.", sourcePath);
        }

        var destinationExists = File.Exists(destinationPath);

        if (destinationExists && !force)
        {
            throw new IOException($"Destination file already exists: {destinationPath}");
        }

        var directory = Path.GetDirectoryName(destinationPath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.Copy(sourcePath, destinationPath, overwrite: force);

        return ValueTask.CompletedTask;
    }

    public ValueTask Move(
        TestDatabaseConnectionString from,
        TestDatabaseConnectionString to,
        bool force,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var sourcePath = pathExtractor.Extract(from);
        var destinationPath = pathExtractor.Extract(to);

        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("Source database file not found.", sourcePath);
        }

        var destinationExists = File.Exists(destinationPath);

        if (destinationExists)
        {
            if (!force)
            {
                throw new IOException($"Destination file already exists: {destinationPath}");
            }

            File.Delete(destinationPath);
        }

        var directory = Path.GetDirectoryName(destinationPath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.Move(sourcePath, destinationPath);

        return ValueTask.CompletedTask;
    }
}