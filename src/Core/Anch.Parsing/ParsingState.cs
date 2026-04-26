namespace Anch.Parsing;

public readonly record struct ParsingState<TInput, TValue>(TValue Value, TInput Rest, bool HasError = false)
{
    public static implicit operator ParsingState<TInput, TValue>(ValueTuple<TValue, TInput> value)
    {
        return new (value.Item1, value.Item2);
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