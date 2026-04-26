namespace Anch.Core;

public static class FuncHelper
{
    public static Func<TResult> Create<TResult>(Func<TResult> func)
    {
        return func;
    }

    public static Func<T, TResult> Create<T, TResult>(Func<T, TResult> func)
    {
        return func;
    }

    public static Func<T1, T2, TResult> Create<T1, T2, TResult>(Func<T1, T2, TResult> func)
    {
        return func;
    }
}