namespace Anch.Core;

public interface IDefaultCancellationTokenSource
{
    CancellationToken CancellationToken { get; }
}