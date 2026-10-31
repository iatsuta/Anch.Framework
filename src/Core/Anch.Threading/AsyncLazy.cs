using System.Runtime.ExceptionServices;

namespace Anch.Threading;

/// <summary>A value that is created asynchronously on first use, once, even when requested concurrently. A failure is remembered and rethrown to every caller; disposing the lazy disposes the value.</summary>
/// <param name="factory">Creates the value.</param>
/// <typeparam name="T">The type of the value.</typeparam>
public class AsyncLazy<T>(Func<CancellationToken, Task<T>> factory) : IAsyncDisposable
{
    private readonly IAsyncLocker asyncLocker = new AsyncLocker();

    private bool disposed;

    private T? value;

    private volatile bool initialized;

    private ExceptionDispatchInfo? exceptionDispatchInfo;

    /// <summary>Returns the value, creating it on the first call.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="ObjectDisposedException">The lazy has been disposed.</exception>
    public async Task<T> GetValueAsync(CancellationToken ct)
    {
        if (!this.initialized)
        {
            using (await this.asyncLocker.CreateScope(ct).ConfigureAwait(false))
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(nameof(AsyncLazy<>));
                }
                else if (!this.initialized)
                {
                    try
                    {
                        this.value = await factory(ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        this.exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                    }
                    finally
                    {
                        this.initialized = true;
                    }
                }
            }
        }

        this.exceptionDispatchInfo?.Throw();

        return this.value!;
    }

    /// <summary><c>true</c> once the factory has finished (successfully or not).</summary>
    public bool Initialized => this.initialized;

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref this.disposed, true))
        {
            return;
        }

        using (this.asyncLocker)
        {
            using (await this.asyncLocker.CreateScope(CancellationToken.None).ConfigureAwait(false))
            {
                if (this.initialized && this.value is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
                else if (this.initialized && this.value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}