namespace Anch.Core;

public static class DefaultCancellationTokenSourceExtensions
{
    public static CancellationToken GetSafeDefaultCancellationTokenSource(this IDefaultCancellationTokenSource? defaultCancellationTokenSource)
    {
        return defaultCancellationTokenSource?.CancellationToken ?? CancellationToken.None;
    }

    public static TResult RunSync<TResult>(this IDefaultCancellationTokenSource? defaultCancellationTokenSource,
        Func<CancellationToken, Task<TResult>> eval)
    {
        return eval(defaultCancellationTokenSource.GetSafeDefaultCancellationTokenSource()).GetAwaiter().GetResult();
    }

    public static void RunSync(this IDefaultCancellationTokenSource? defaultCancellationTokenSource,
        Func<CancellationToken, Task> eval)
    {
        eval(defaultCancellationTokenSource.GetSafeDefaultCancellationTokenSource()).GetAwaiter().GetResult();
    }
}