using System.Diagnostics;

namespace CommonFramework;

public static class PipeMaybeObjectExtensions
{
    [DebuggerStepThrough]
    public static void Maybe<TSource>(this TSource? source, Action<TSource> evaluate)
        where TSource : class
    {
        if (null != source)
        {
            evaluate(source);
        }
    }

    [DebuggerStepThrough]
    public static TResult? Maybe<TSource, TResult>(this TSource? source, Func<TSource, TResult> selector)
    {
        return null == source ? default : selector(source);
    }

    public static void MaybeNullable<TSource>(this TSource? source, Action<TSource> evaluate)
        where TSource : struct
    {
        if (null != source)
        {
            evaluate(source.Value);
        }
    }

    public static TResult MaybeNullable<TSource, TResult>(this TSource? source, Func<TSource, TResult> selector,
        TResult nullableResult = default!)
        where TSource : struct
    {
        return null == source ? nullableResult : selector(source.Value);
    }

    public static TResult? MaybeNullableToNullable<TSource, TResult>(this TSource? source,
        Func<TSource, TResult> selector)
        where TSource : struct
        where TResult : struct
    {
        return null == source ? null : selector(source.Value);
    }

    public static TResult? Maybe<TSource, TResult>(this TSource? source, Func<TSource, bool> condition,
        Func<TSource, TResult> getResult)
        where TSource : class
    {
        return source.Maybe(v => condition(v) ? getResult(v) : default);
    }

    [DebuggerStepThrough]
    public static TResult? MaybeToNullable<TSource, TResult>(this TSource? source, Func<TSource, TResult> selector)
        where TSource : class
        where TResult : struct
    {
        return null == source ? null : selector(source);
    }

    [DebuggerStepThrough]
    public static TResult Maybe<TObject, TResult>(this TObject? source, Func<TObject, TResult> selector,
        TResult ifNullResult)
        where TObject : class
    {
        return null == source ? ifNullResult : selector(source);
    }

    [DebuggerStepThrough]
    public static TResult Maybe<TObject, TResult>(this TObject? source, Func<TObject, TResult> selector,
        Func<TResult> ifNullResult)
        where TObject : class
    {
        return null == source ? ifNullResult() : selector(source);
    }

    public static void Maybe<TObject>(this TObject? source, Func<TObject, bool> condition, Action<TObject> action)
        where TObject : class
    {
        if (null != source && condition(source))
        {
            action(source);
        }
    }
}