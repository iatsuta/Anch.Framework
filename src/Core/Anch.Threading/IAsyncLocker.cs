namespace Anch.Threading;

/// <summary>An asynchronous mutual-exclusion lock.</summary>
public interface IAsyncLocker : IDisposable
{
    /// <summary>Waits for the lock and returns a scope that releases it when disposed.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task<IDisposable> CreateScope(CancellationToken ct);
}