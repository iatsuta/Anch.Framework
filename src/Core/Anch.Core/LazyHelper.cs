namespace Anch.Core;

public static class LazyHelper
{
    public static Lazy<TResult> Create<TResult>(Func<TResult> func)
    {
        return new Lazy<TResult>(func);
    }
}