namespace CommonFramework.Testing;

public interface IDatabaseGenerator
{
    Task CreateEmptyAsync(CancellationToken cancellationToken);

    Task InitializeDataAsync(CancellationToken cancellationToken);
}