using System.Runtime.ExceptionServices;

namespace Anch.Threading;

public class AsyncOnce(Func<CancellationToken, Task> action)
{
    private readonly IAsyncLocker asyncLocker = new AsyncLocker();

    private volatile bool executed;

    private ExceptionDispatchInfo? exceptionDispatchInfo;

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

    public bool Executed => this.executed;
}