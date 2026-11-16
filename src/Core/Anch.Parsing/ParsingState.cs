namespace Anch.Parsing;

/// <summary>The result of a parser step: the parsed value, the rest of the input and whether an error occurred.</summary>
/// <param name="Value">The parsed value.</param>
/// <param name="Rest">The input that remains after the parsed part.</param>
/// <param name="HasError">Whether the parser did not match.</param>
/// <typeparam name="TInput">The type of the parsed input.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
public readonly record struct ParsingState<TInput, TValue>(TValue Value, TInput Rest, bool HasError = false)
{
    public static implicit operator ParsingState<TInput, TValue>(ValueTuple<TValue, TInput> value)
    {
        return new(value.Item1, value.Item2);
    }

    public ParsingState<TInput, TValue> ToError()
    {
        return this with { HasError = true };
    }

    public ParsingState<TInput, TNewValue> ToError<TNewValue>()
    {
        return new ParsingState<TInput, TNewValue>(default!, this.Rest, true);
    }
}