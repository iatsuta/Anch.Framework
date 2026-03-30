namespace CommonFramework.Parsing;

public delegate ParsingState<TInput, TValue> Parser<TInput, TValue>(TInput input);