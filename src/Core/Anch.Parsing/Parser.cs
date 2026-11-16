namespace Anch.Parsing;

/// <summary>Parser helpers for a given input type.</summary>
/// <typeparam name="TInput">The type of the parsed input.</typeparam>
public static class Parser<TInput>
{
    /// <summary>A parser that consumes nothing and always succeeds with the given value.</summary>
    /// <param name="value">The result value.</param>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public static Parser<TInput, TValue> Return<TValue>(TValue value)
    {
        return input => new ParsingState<TInput, TValue>(value, input);
    }
}

/// <summary>Parser helpers.</summary>
public static class Parser
{
    /// <summary>Creates a parser lazily, which allows recursive grammars.</summary>
    /// <param name="getValue">Returns the parser; called each time the parser runs.</param>
    /// <typeparam name="TInput">The type of the parsed input.</typeparam>
    /// <typeparam name="TValue">The type of the result.</typeparam>
    public static Parser<TInput, TValue> Return<TInput, TValue>(Func<Parser<TInput, TValue>> getValue)
    {
        return input => getValue().Invoke(input);
    }
}

/// <summary>A parser: takes the input and returns the parsed value with the remaining input, or an error state. Parsers are combined with LINQ query syntax and the combinators of <c>Parsers</c>.</summary>
/// <param name="input">The input to parse.</param>
/// <typeparam name="TInput">The type of the parsed input.</typeparam>
/// <typeparam name="TValue">The type of the result.</typeparam>
public delegate ParsingState<TInput, TValue> Parser<TInput, TValue>(TInput input);