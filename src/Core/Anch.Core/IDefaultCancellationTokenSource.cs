namespace Anch.Core;

/// <summary>Supplies a cancellation token for code that cannot receive one as a parameter (for example synchronous code that calls async APIs).</summary>
public interface IDefaultCancellationTokenSource
{
    /// <summary>The token to use.</summary>
    CancellationToken CancellationToken { get; }
}