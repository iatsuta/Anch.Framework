namespace CommonFramework.Parsing;

public static class ParserCombinatorExtensions
{
    extension<TInput, TValue>(Parser<TInput, TValue> parser)
    {
        public Parser<TInput, TValue> Or(Func<Parser<TInput, TValue>> getOtherParser) =>

            parser.Or(Parser.Return(getOtherParser));

        public Parser<TInput, TValue> Or(Parser<TInput, TValue> otherParser)
        {
            return input =>
            {
                var res1 = parser(input);

                if (res1.HasError)
                {
                    var res2 = otherParser(input);

                    if (res2.HasError)
                    {
                        if (Comparer<TInput>.Default.Compare(res1.Rest, res2.Rest) <= 0)
                        {
                            return res1;
                        }
                        else
                        {
                            return res2;
                        }
                    }
                    else
                    {
                        return res2;
                    }
                }
                else
                {
                    return res1;
                }
            };
        }

        public Parser<TInput, TNextValue> Pipe<TNextValue>(Parser<TInput, Func<TValue, TNextValue>> otherParser) =>
            from v in parser
            from f in otherParser
            select f(v);

        public Parser<TInput, object> Box() =>
            from v in parser
            select (object)v;

        public ParserTableRow<TInput, TValue> ToRow(Func<TValue> getDefaultValue) => new(parser, getDefaultValue);
    }

    public static Parser<TInput, Func<TArg, TResultValue>> Compose<TInput, TArg, TValue, TResultValue>(
        this Parser<TInput, Func<TArg, TValue>> parser,
        Parser<TInput, Func<TValue, TResultValue>> otherParser) =>
        from f1 in parser
        from f2 in otherParser
        select new Func<TArg, TResultValue>(v => f2(f1(v)));
}