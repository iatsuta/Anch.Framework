namespace Anch.Threading;

public interface IAsyncLocker : IDisposable
{
    Task<IDisposable> CreateScope(CancellationToken ct);
}