// ReSharper disable once CheckNamespace
namespace CommonFramework;

public readonly struct Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T value;

    public readonly bool HasValue;

    private Maybe(T value, bool hasValue)
    {
        this.value = value;
        this.HasValue = hasValue;
    }

    public T Value => !this.HasValue ? throw new InvalidOperationException("No value present") : this.value;

    public static Maybe<T> Nothing => new (default!, false);

    public static Maybe<T> Just(T value) => new (value, true);

    public override string ToString() => this.HasValue ? $"{this.value}" : "";

    public bool Equals(Maybe<T> other) => this.HasValue == other.HasValue && EqualityComparer<T>.Default.Equals(this.value, other.value);

    public override bool Equals(object? obj) => obj is Maybe<T> other && this.Equals(other);

    public override int GetHashCode() => HashCode.Combine(this.value, this.HasValue);

    public static bool operator ==(Maybe<T> maybe, Maybe<T> otherMaybe) => maybe.Equals(otherMaybe);

    public static bool operator !=(Maybe<T> maybe, Maybe<T> otherMaybe) => !(maybe == otherMaybe);
}

public static class Maybe
{
    public static Maybe<Ignore> Return()
    {
        return Return(Ignore.Value);
    }

    public static Maybe<T> ToMaybe<T>(this T? value)
        where T : struct
    {
        return OfCondition(value != null, () => value!.Value);
    }

    public static Maybe<T> Return<T>(T value)
    {
        return Maybe<T>.Just(value);
    }

    public static Maybe<T> ToMaybe<T>(this T? value)
        where T : class
    {
        return OfCondition(value != null, () => value!);
    }

    public static Maybe<T> OfCondition<T>(bool condition, Func<T> getJustValue)
    {
        return OfCondition(condition, () => Return(getJustValue()), () => Maybe<T>.Nothing);
    }

    public static Maybe<T> OfCondition<T>(bool condition, Func<Maybe<T>> getTrueValue, Func<Maybe<T>> getFalseValue)
    {
        return condition ? getTrueValue() : getFalseValue();
    }

    public static Func<TArg, Maybe<TResult>> OfTryMethod<TArg, TResult>(TryMethod<TArg, TResult> tryAction)
    {
        return arg =>
        {
            return OfCondition(tryAction(arg, out var result), () => result);
        };
    }

    public static Func<TArg1, TArg2, Maybe<TResult>> OfTryMethod<TArg1, TArg2, TResult>(TryMethod<TArg1, TArg2, TResult> tryAction)
    {
        return (arg1, arg2) =>
        {
            return OfCondition(tryAction(arg1, arg2, out var result), () => result);
        };
    }

    public delegate bool TryMethod<in TArg, TResult>(TArg arg, out TResult result);

    public delegate bool TryMethod<in TArg1, in TArg2, TResult>(TArg1 arg1, TArg2 arg2, out TResult result);
}