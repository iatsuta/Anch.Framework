namespace Anch.Threading;

public interface IAsyncLocker
{
    ValueTask<IDisposable> CreateScope(CancellationToken ct = default);
}