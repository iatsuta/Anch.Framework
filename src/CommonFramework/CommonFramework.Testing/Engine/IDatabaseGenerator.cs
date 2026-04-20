namespace CommonFramework.Testing.Engine;

public interface IDatabaseGenerator
{
    Task CreateEmptyAsync(CancellationToken cancellationToken);

    Task InitializeDataAsync(CancellationToken cancellationToken);
}