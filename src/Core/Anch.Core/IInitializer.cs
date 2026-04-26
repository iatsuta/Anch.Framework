namespace Anch.Core;

public interface IInitializer
{
    Task Initialize(CancellationToken cancellationToken = default);
}