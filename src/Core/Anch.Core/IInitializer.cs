namespace Anch.Core;

public interface IInitializer
{
    Task Initialize(CancellationToken ct);
}