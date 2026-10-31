// ReSharper disable once CheckNamespace
namespace Anch.Core;

/// <summary>An optional value: either <see cref="Just"/> a value or <see cref="Nothing"/>. Unlike <c>null</c> it works for value types and makes absence explicit.</summary>
/// <typeparam name="T">The type of the value.</typeparam>
public readonly struct Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T value;

    /// <summary><c>true</c> if a value is present.</summary>
    public readonly bool HasValue;

    private Maybe(T value, bool hasValue)
    {
        this.value = value;
        this.HasValue = hasValue;
    }

    /// <summary>The value.</summary>
    /// <exception cref="InvalidOperationException">No value is present.</exception>
    public T Value => !this.HasValue ? throw new InvalidOperationException("No value present") : this.value;

    /// <summary>The absence of a value.</summary>
    public static Maybe<T> Nothing => new(default!, false);

    /// <summary>Wraps a value.</summary>
    /// <param name="value">The value.</param>
    public static Maybe<T> Just(T value) => new(value, true);

    public override string ToString() => this.HasValue ? $"{this.value}" : "";

    public bool Equals(Maybe<T> other) => this.HasValue == other.HasValue && EqualityComparer<T>.Default.Equals(this.value, other.value);

    public override bool Equals(object? obj) => obj is Maybe<T> other && this.Equals(other);

    public override int GetHashCode() => HashCode.Combine(this.value, this.HasValue);

    public static bool operator ==(Maybe<T> maybe, Maybe<T> otherMaybe) => maybe.Equals(otherMaybe);

    public static bool operator !=(Maybe<T> maybe, Maybe<T> otherMaybe) => !(maybe == otherMaybe);
}

/// <summary>Helpers for creating <see cref="Maybe{T}"/> values.</summary>
public static class Maybe
{
    /// <summary>Creates a value that carries no data, only the fact that it is present.</summary>
    public static Maybe<Ignore> Return()
    {
        return Return(Ignore.Value);
    }

    /// <summary>Converts a nullable value type: <see cref="Maybe{T}.Nothing"/> for <c>null</c>.</summary>
    /// <param name="value">The nullable value.</param>
    /// <typeparam name="T">The value type.</typeparam>
    public static Maybe<T> ToMaybe<T>(this T? value)
        where T : struct
    {
        return OfCondition(value is not null, () => value!.Value);
    }

    /// <summary>Wraps a value.</summary>
    /// <param name="value">The value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    public static Maybe<T> Return<T>(T value)
    {
        return Maybe<T>.Just(value);
    }

    /// <summary>Converts a reference: <see cref="Maybe{T}.Nothing"/> for <c>null</c>.</summary>
    /// <param name="value">The reference.</param>
    /// <typeparam name="T">The reference type.</typeparam>
    public static Maybe<T> ToMaybe<T>(this T? value)
        where T : class
    {
        return OfCondition(value is not null, () => value!);
    }

    /// <summary>Returns the value from <paramref name="getJustValue"/> when the condition holds, otherwise <see cref="Maybe{T}.Nothing"/>.</summary>
    /// <param name="condition">The condition.</param>
    /// <param name="getJustValue">Produces the value; called only when the condition holds.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    public static Maybe<T> OfCondition<T>(bool condition, Func<T> getJustValue)
    {
        return OfCondition(condition, () => Return(getJustValue()), () => Maybe<T>.Nothing);
    }

    /// <summary>Returns one of two optional values depending on the condition.</summary>
    /// <param name="condition">The condition.</param>
    /// <param name="getTrueValue">Produces the result when the condition holds.</param>
    /// <param name="getFalseValue">Produces the result otherwise.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    public static Maybe<T> OfCondition<T>(bool condition, Func<Maybe<T>> getTrueValue, Func<Maybe<T>> getFalseValue)
    {
        return condition ? getTrueValue() : getFalseValue();
    }

    /// <summary>Adapts a <c>TryXxx(arg, out result)</c> method into a function that returns a <see cref="Maybe{T}"/>.</summary>
    /// <param name="tryAction">The <c>Try</c> method.</param>
    /// <typeparam name="TArg">The argument type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public static Func<TArg, Maybe<TResult>> OfTryMethod<TArg, TResult>(TryMethod<TArg, TResult> tryAction)
    {
        return arg =>
        {
            return OfCondition(tryAction(arg, out var result), () => result);
        };
    }

    /// <summary>Adapts a <c>TryXxx(arg1, arg2, out result)</c> method into a function that returns a <see cref="Maybe{T}"/>.</summary>
    /// <param name="tryAction">The <c>Try</c> method.</param>
    /// <typeparam name="TArg1">The type of the first argument.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public static Func<TArg1, TArg2, Maybe<TResult>> OfTryMethod<TArg1, TArg2, TResult>(TryMethod<TArg1, TArg2, TResult> tryAction)
    {
        return (arg1, arg2) =>
        {
            return OfCondition(tryAction(arg1, arg2, out var result), () => result);
        };
    }

    /// <summary>A <c>TryXxx(arg, out result)</c> method.</summary>
    /// <param name="arg">The argument.</param>
    /// <param name="result">The result when the method succeeds.</param>
    /// <typeparam name="TArg">The argument type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public delegate bool TryMethod<in TArg, TResult>(TArg arg, out TResult result);

    /// <summary>A <c>TryXxx(arg1, arg2, out result)</c> method.</summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="result">The result when the method succeeds.</param>
    /// <typeparam name="TArg1">The type of the first argument.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public delegate bool TryMethod<in TArg1, in TArg2, TResult>(TArg1 arg1, TArg2 arg2, out TResult result);
}