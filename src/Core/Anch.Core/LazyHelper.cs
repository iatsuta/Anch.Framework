namespace Anch.Core;

/// <summary>Helpers for creating lazy values.</summary>
public static class LazyHelper
{
    public static Lazy<TResult> Create<TResult>(Func<TResult> func)
    {
        return new Lazy<TResult>(func);
    }
}