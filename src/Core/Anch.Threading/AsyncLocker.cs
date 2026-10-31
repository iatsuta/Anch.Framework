namespace Anch.Threading;

/// <summary>An asynchronous mutual-exclusion lock based on a semaphore.</summary>
public class AsyncLocker : IAsyncLocker
{
    private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

    public async Task<IDisposable> CreateScope(CancellationToken ct)
    {
        await this.semaphoreSlim.WaitAsync(ct).ConfigureAwait(false);

        return new AsyncLockerScope(this.semaphoreSlim);
    }

    private class AsyncLockerScope(SemaphoreSlim semaphoreSlim) : IDisposable
    {
        public void Dispose() => semaphoreSlim.Release();
    }

    public void Dispose() => this.semaphoreSlim.Dispose();
}