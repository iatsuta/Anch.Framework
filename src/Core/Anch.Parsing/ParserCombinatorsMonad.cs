namespace Anch.Parsing;

public static class ParserCombinatorsMonad
{
    public static Parser<TInput, TValue> Where<TInput, TValue>(
        this Parser<TInput, TValue> parser,
        Func<TValue, bool> predicate)
    {
        return input =>
        {
            var res = parser(input);

            if (res.Value == null)
            {
                return res;
            }
            else
            {
                if (predicate(res.Value))
                {
                    return res;
                }
                else
                {
                    return res.ToError();
                }
            }
        };
    }

    public static Parser<TInput, TNewValue> Select<TInput, TValue, TNewValue>(
        this Parser<TInput, TValue> parser,
        Func<TValue, TNewValue> selector)
    {
        return input =>
        {
            var res = parser(input);

            if (res.HasError)
            {
                return res.ToError<TNewValue>();
            }
            else
            {
                var newValue = selector(res.Value);

                return new ParsingState<TInput, TNewValue>(newValue, res.Rest);
            }
        };
    }

    public static Parser<TInput, TNewValue> SelectMany<TInput, TValue, TIntermediate, TNewValue>(
        this Parser<TInput, TValue> parser,
        Func<TValue, Parser<TInput, TIntermediate>> selector,
        Func<TValue, TIntermediate, TNewValue> projector)
    {
        return input =>
        {
            var res1 = parser(input);

            if (res1.HasError)
            {
                return res1.ToError<TNewValue>();
            }
            else
            {
                var val1 = res1.Value;

                var intermediateParser = selector(val1);

                var intermediateRes = intermediateParser(res1.Rest);

                if (intermediateRes.HasError)
                {
                    return intermediateRes.ToError<TNewValue>();
                }
                else
                {
                    return new ParsingState<TInput, TNewValue>(
                        projector(val1, intermediateRes.Value),
                        intermediateRes.Rest);
                }
            }
        };
    }
}