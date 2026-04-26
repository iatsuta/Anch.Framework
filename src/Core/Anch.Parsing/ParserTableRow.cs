namespace Anch.Parsing;

public record ParserTableRow<TInput, TValue>(Parser<TInput, TValue> Parser, Func<TValue> GetDefaultValue);