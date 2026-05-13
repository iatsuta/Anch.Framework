namespace Anch.Threading;

public class AsyncLocker : IAsyncLocker
{
    private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

    public async ValueTask<IDisposable> CreateScope(CancellationToken ct)
    {
        await this.semaphoreSlim.WaitAsync(ct).ConfigureAwait(false);

        return new AsyncLockerScope(this.semaphoreSlim);
    }

    private readonly struct AsyncLockerScope(SemaphoreSlim semaphoreSlim) : IDisposable
    {
        public void Dispose() => semaphoreSlim.Release();
    }
}