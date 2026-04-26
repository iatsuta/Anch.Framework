// ReSharper disable once CheckNamespace
namespace Anch.Core;

public static class MaybeExtensions
{
    extension<TSource>(Maybe<TSource> maybeValue)
    {
        public TResult Match<TResult>(Func<TSource, TResult> fromJustResult, Func<TResult> fromNothingResult)
        {
            return maybeValue.HasValue ? fromJustResult(maybeValue.Value) : fromNothingResult();
        }

        public void Match(Action<TSource> fromJustAction, Action? fromNothingAction = null)
        {
            if (maybeValue.HasValue)
            {
                fromJustAction(maybeValue.Value);
            }
            else
            {
                fromNothingAction?.Invoke();
            }
        }

        public TSource GetValue()
        {
            return maybeValue.GetValue(() => new Exception("Nothing Value"));
        }

        public TSource GetValue(Func<Exception> nothingException)
        {
            return maybeValue.Match(result => result, () => throw nothingException());
        }

        public TSource? GetValueOrDefault()
        {
            return maybeValue.GetValueOrDefault(default(TSource));
        }
    }

    public static Maybe<TResult> Or<TSource, TResult>(this Maybe<TSource> v1, Func<Maybe<TResult>> getV2)
        where TSource : TResult
    {
        return v1.HasValue ? v1.Select(v => (TResult)v) : getV2();
    }

    public static TResult GetValueOrDefault<TSource, TResult>(this Maybe<TSource> maybeValue, TResult defaultValue)
        where TSource : TResult
    {
        return maybeValue.Select(v => (TResult)v).GetValueOrDefault(() => defaultValue);
    }

    public static TResult GetValueOrDefault<TSource, TResult>(this Maybe<TSource> maybeValue, Func<TResult> getDefaultValue)
        where TSource : TResult
    {
        return maybeValue.Match(result => result, getDefaultValue);
    }

    extension(Maybe<bool> v1)
    {
        public Maybe<bool> LogicOr(Func<Maybe<bool>> getV2)
        {

            return v1.Where(p => p).Or(() => LogicOrDict[v1][getV2()]);
        }

        public Maybe<bool> LogicOr(Maybe<bool> v2)
        {
            return v1.LogicOr(() => v2);
        }

        public Maybe<bool> LogicAnd(Func<Maybe<bool>> getV2)
        {
            return v1.Where(p => !p).Or(() => LogicAndDict[v1][getV2()]);
        }
    }

    private static readonly Dictionary<Maybe<bool>, Dictionary<Maybe<bool>, Maybe<bool>>> LogicOrDict = new()
    {
        //{ Maybe.Return(true), new Dictionary<Maybe<bool>, Maybe<bool>>
        //                      {
        //                          { Maybe.Return(true), Maybe.Return(true) },
        //                          { Maybe.Return(false), Maybe.Return(true) },
        //                          { Maybe<bool>.Nothing, Maybe.Return(true) }
        //                      } },

        { Maybe.Return(false), new Dictionary<Maybe<bool>, Maybe<bool>>
        {
            { Maybe.Return(true), Maybe.Return(true) },
            { Maybe.Return(false), Maybe.Return(false) },
            { Maybe<bool>.Nothing, Maybe<bool>.Nothing }
        } },

        {  Maybe<bool>.Nothing, new Dictionary<Maybe<bool>, Maybe<bool>>
        {
            { Maybe.Return(true), Maybe.Return(true) },
            { Maybe.Return(false), Maybe<bool>.Nothing },
            { Maybe<bool>.Nothing, Maybe<bool>.Nothing }
        } },
    };


    private static readonly Dictionary<Maybe<bool>, Dictionary<Maybe<bool>, Maybe<bool>>> LogicAndDict = new()
    {
        { Maybe.Return(true), new Dictionary<Maybe<bool>, Maybe<bool>>
        {
            { Maybe.Return(true), Maybe.Return(true) },
            { Maybe.Return(false), Maybe.Return(false) },
            { Maybe<bool>.Nothing, Maybe<bool>.Nothing }
        } },

        //{ Maybe.Return(false), new Dictionary<Maybe<bool>, Maybe<bool>>
        //                       {
        //                           { Maybe.Return(true), Maybe.Return(false) },
        //                           { Maybe.Return(false), Maybe.Return(false) },
        //                           { Maybe<bool>.Nothing, Maybe.Return(false) }
        //                       } },

        {  Maybe<bool>.Nothing, new Dictionary<Maybe<bool>, Maybe<bool>>
        {
            { Maybe.Return(true), Maybe<bool>.Nothing },
            { Maybe.Return(false), Maybe.Return(false) },
            { Maybe<bool>.Nothing, Maybe<bool>.Nothing }
        } },
    };
}