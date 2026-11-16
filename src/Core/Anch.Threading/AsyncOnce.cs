using System.Runtime.ExceptionServices;

namespace Anch.Threading;

/// <summary>Runs an asynchronous action exactly once, even when called concurrently. If the action throws, every caller gets the same exception.</summary>
/// <param name="action">The action to run once.</param>
public class AsyncOnce(Func<CancellationToken, Task> action)
{
    private readonly IAsyncLocker asyncLocker = new AsyncLocker();

    private volatile bool executed;

    private ExceptionDispatchInfo? exceptionDispatchInfo;

    /// <summary>Runs the action if it has not run yet and waits for it to finish; rethrows its exception, if any.</summary>
    /// <param name="ct">Cancellation token.</param>
    public async Task EnsureExecutedAsync(CancellationToken ct)
    {
        if (!this.executed)
        {
            using (await this.asyncLocker.CreateScope(ct).ConfigureAwait(false))
            {
                if (!this.executed)
                {
                    try
                    {
                        await action(ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        this.exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                    }
                    finally
                    {
                        this.executed = true;
                    }
                }
            }
        }

        this.exceptionDispatchInfo?.Throw();
    }

    /// <summary><c>true</c> once the action has finished (successfully or not).</summary>
    public bool Executed => this.executed;
}