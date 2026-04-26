// ReSharper disable once CheckNamespace
namespace Anch.Core;

public static class LinqMaybeExtensions
{
    public static Maybe<TResult> Select<TSource, TResult>(this Maybe<TSource> source, Func<TSource, TResult> selector)
    {
        return source.Match(result => Maybe.Return(selector(result)), () => Maybe<TResult>.Nothing);
    }

    public static Maybe<TResult> SelectMany<TSource, TNextResult, TResult>(this Maybe<TSource> source, Func<TSource, Maybe<TNextResult>> nextSelector,
        Func<TSource, TNextResult, TResult> resultSelector)
    {
        return source.Match(result1 => nextSelector(result1).Match(result2 => Maybe.Return(resultSelector(result1, result2)),
                () => Maybe<TResult>.Nothing),
            () => Maybe<TResult>.Nothing);
    }

    public static Maybe<TResult> SelectMany<TSource, TResult>(this Maybe<TSource> source, Func<TSource, Maybe<TResult>> nextSelector)
    {
        return source.SelectMany(nextSelector, (_, res) => res);
    }

    public static Maybe<T> Where<T>(this Maybe<T> source, Func<T, bool> filter)
    {
        return source.Match(result => Maybe.OfCondition(filter(result), () => result), () => Maybe<T>.Nothing);
    }
}