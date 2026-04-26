namespace Anch.Parsing;

public static class Parser<TInput>
{
    public static Parser<TInput, TValue> Return<TValue>(TValue value)
    {
        return input => new ParsingState<TInput, TValue>(value, input);
    }
}

public static class Parser
{
    public static Parser<TInput, TValue> Return<TInput, TValue>(Func<Parser<TInput, TValue>> getValue)
    {
        return input => getValue().Invoke(input);
    }
}

public delegate ParsingState<TInput, TValue> Parser<TInput, TValue>(TInput input);