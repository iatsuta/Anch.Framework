namespace Anch.Core;

public static class PipeObjectExtensions
{
    public static TSource PipeMaybe<TSource, TValue>(this TSource source, TValue? value, Func<TSource, TValue, TSource> evaluate)
        where TValue : struct
    {
        return value == null ? source : evaluate(source, value.Value);
    }

    public static TSource PipeMaybe<TSource, TValue>(this TSource source, TValue? value, Func<TSource, TValue, TSource> evaluate)
        where TValue : class
    {
        return value == null ? source : evaluate(source, value);
    }

    public static TResult Pipe<TSource, TResult>(this TSource source, Func<TSource, TResult> evaluate)
    {
        return evaluate(source);
    }

    public static TResult Pipe<TSource, TResult>(this TSource source, bool condition, Func<TSource, TResult> evaluate)
        where TSource : TResult
    {
        return condition ? evaluate(source) : source;
    }

    public static void Pipe<TSource>(this TSource source, Action<TSource> evaluate)
    {
        evaluate(source);
    }

    public static TSource FromMaybe<TSource>(this TSource? source, Func<string> getNothingExceptionMessage)
        where TSource : class
    {
        return source.FromMaybe(() => new Exception(getNothingExceptionMessage()));
    }

    public static TSource FromMaybe<TSource>(this TSource? source, Func<Exception> getNothingException)
        where TSource : class
    {
        return source ?? throw getNothingException();
    }
}