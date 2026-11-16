namespace Anch.Parsing;

/// <summary>A parser together with the value to use when it does not match; a row of a parser table.</summary>
public record ParserTableRow<TInput, TValue>(Parser<TInput, TValue> Parser, Func<TValue> GetDefaultValue);