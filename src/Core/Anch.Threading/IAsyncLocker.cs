namespace Anch.Threading;

public interface IAsyncLocker
{
    Task<IDisposable> CreateScope(CancellationToken ct = default);
}