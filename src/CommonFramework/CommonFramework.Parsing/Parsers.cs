using System.Collections.Immutable;

namespace CommonFramework.Parsing;

public abstract class Parsers<TInput>
{
    protected Parser<TInput, TResult> OfTable<T1, T2, T3, T4, T5, T6, TSeparator, TResult>(
        ParserTableRow<TInput, T1> p1,
        ParserTableRow<TInput, T2> p2,
        ParserTableRow<TInput, T3> p3,
        ParserTableRow<TInput, T4> p4,
        ParserTableRow<TInput, T5> p5,
        ParserTableRow<TInput, T6> p6,
        Parser<TInput, TSeparator> separator,
        Func<T1, T2, T3, T4, T5, T6, TResult> resultSelector)
    {
        var table = new Dictionary<string, Parser<TInput, object>>
        {
            { "p1", p1.Parser.Box() },
            { "p2", p2.Parser.Box() },
            { "p3", p3.Parser.Box() },
            { "p4", p4.Parser.Box() },
            { "p5", p5.Parser.Box() },
            { "p6", p6.Parser.Box() },
        };


        return from processResult in this.SubOfTable(table, separator)

            let v1 = (T1)processResult.GetValueOrDefault("p1", () => p1.GetDefaultValue())

            let v2 = (T2)processResult.GetValueOrDefault("p2", () => p2.GetDefaultValue())

            let v3 = (T3)processResult.GetValueOrDefault("p3", () => p3.GetDefaultValue())

            let v4 = (T4)processResult.GetValueOrDefault("p4", () => p4.GetDefaultValue())

            let v5 = (T5)processResult.GetValueOrDefault("p5", () => p5.GetDefaultValue())

            let v6 = (T6)processResult.GetValueOrDefault("p6", () => p6.GetDefaultValue())

            select resultSelector(v1, v2, v3, v4, v5, v6);
    }


    private Parser<TInput, Dictionary<string, object>> SubOfTable<TSeparator>(Dictionary<string, Parser<TInput, object>> table,
        Parser<TInput, TSeparator> separator)
    {
        var successRowParser = this.OneOfMany(table.Select(pair => pair.Value.Select(value => new KeyValuePair<string, object>(pair.Key, value))));

        return (from rowPair in successRowParser

            from subParseResult in (from sep in separator

                from subPairs in this.SubOfTable(table.Where(pair => pair.Key != rowPair.Key).ToDictionary(), separator)

                select subPairs).Or(() => this.Return(new Dictionary<string, object>()))

            select new[] { rowPair }.ToDictionary().Concat(subParseResult)).Or(() => this.Return(new Dictionary<string, object>()));
    }

    protected Parser<TInput, TValue> MaybeParser<TValue>(Func<Maybe.Maybe<TValue>> getValue)
    {
        var value = getValue();

        if (value.HasValue)
        {
            return this.Return(value.Value);
        }
        else
        {
            return this.Fault<TValue>();
        }
    }

    protected Parser<TInput, TValue> CatchParser<TValue>(Func<TValue> createFunc)
    {
        try
        {
            return this.Return(createFunc());
        }
        catch
        {
            return this.Fault<TValue>();
        }
    }

    public Parser<TInput, Ignore> Fault()
    {
        return this.Fault<Ignore>();
    }

    public Parser<TInput, TValue> Fault<TValue>()
    {
        return _ => new ParsingState<TInput, TValue>(default!, default!, true);
    }

    public Parser<TInput, Ignore> Return()
    {
        return Parser<TInput>.Return(Ignore.Value);
    }

    public Parser<TInput, Func<TIdentity, TIdentity>> GetIdentityFunc<TIdentity>()
    {
        return this.Return(new Func<TIdentity, TIdentity>(v => v));
    }

    public Parser<TInput, Func<TResult, TIdentity>> GetIdentityFunc<TIdentity, TResult>()
        where TResult : TIdentity
    {
        return this.Return(new Func<TResult, TIdentity>(v => v));
    }

    public Parser<TInput, TValue> Return<TValue>(TValue value)
    {
        return input => new ParsingState<TInput, TValue>(value, input);
    }

    public Parser<TInput, TValue> OneOfMany<TValue>(params Parser<TInput, TValue>[] parsers)
    {
        if (parsers == null) throw new ArgumentNullException(nameof(parsers));

        return this.OneOfMany((IEnumerable<Parser<TInput, TValue>>)parsers);
    }

    public Parser<TInput, TValue> OneOfMany<TValue>(IEnumerable<Parser<TInput, TValue>> parsers)
    {
        if (parsers == null) throw new ArgumentNullException(nameof(parsers));

        return parsers.Select(p => this.GetLazy(() => p)).Aggregate((p1, p2) => p1.Or(p2));
    }


    public Parser<TInput, ImmutableArray<TValue>> Many<TValue>(Parser<TInput, TValue> parser)
    {
        return this.Many1(parser).Or(this.Return(ImmutableArray<TValue>.Empty));
    }

    public Parser<TInput, ImmutableArray<TValue>> Many1<TValue>(Parser<TInput, TValue> parser)
    {
        return input =>
        {
            var prevResult = parser(input);

            if (prevResult.HasError)
            {
                return prevResult.ToError<ImmutableArray<TValue>>();
            }
            else
            {
                var builder = ImmutableArray.CreateBuilder<TValue>();
                builder.Add(prevResult.Value);

                while (true)
                {
                    var nextResult = parser(prevResult.Rest);

                    if (!nextResult.HasError)
                    {
                        prevResult = nextResult;
                        builder.Add(prevResult.Value);
                    }
                    else
                    {
                        return new ParsingState<TInput, ImmutableArray<TValue>>(builder.ToImmutable(), prevResult.Rest);
                    }
                }
            }
        };
    }

    public Parser<TInput, Ignore> TestYes<TValue>(Parser<TInput, TValue> testParser)
    {
        return input =>
        {
            var prevResult = testParser(input);

            if (prevResult.HasError)
            {
                return prevResult.ToError<Ignore>();
            }
            else
            {
                return new ParsingState<TInput, Ignore>(Ignore.Value, input);
            }
        };
    }

    public Parser<TInput, Ignore> TestNo<TValue>(Parser<TInput, TValue> testParser)
    {
        return input =>
        {
            var prevResult = testParser(input);

            if (prevResult.HasError)
            {
                return new ParsingState<TInput, Ignore>(Ignore.Value, input);
            }
            else
            {
                return prevResult.ToError<Ignore>();
            }
        };
    }

    public Parser<TInput, ImmutableArray<TValue>> SepBy<TValue, TSeparator>(Parser<TInput, TValue> parser, Parser<TInput, TSeparator> separatorParser)
    {
        return this.SepBy1(parser, separatorParser).Or(this.Return(ImmutableArray<TValue>.Empty));
    }

    public Parser<TInput, ImmutableArray<TValue>> SepBy1<TValue, TSeparator>(Parser<TInput, TValue> parser, Parser<TInput, TSeparator> separatorParser)
    {
        return input =>
        {
            var prevResult = parser(input);

            if (prevResult.HasError)
            {
                return prevResult.ToError<ImmutableArray<TValue>>();
            }
            else
            {
                var builder = ImmutableArray.CreateBuilder<TValue>();
                builder.Add(prevResult.Value);

                while (true)
                {
                    var afterSeparatorResult = separatorParser(prevResult.Rest);

                    if (!afterSeparatorResult.HasError)
                    {
                        var nextResult = parser(afterSeparatorResult.Rest);

                        if (!nextResult.HasError)
                        {
                            prevResult = nextResult;
                            builder.Add(prevResult.Value);
                        }
                        else
                        {
                            return new ParsingState<TInput, ImmutableArray<TValue>>(builder.ToImmutable(), prevResult.Rest);
                        }
                    }
                    else
                    {
                        return new ParsingState<TInput, ImmutableArray<TValue>>(builder.ToImmutable(), prevResult.Rest);
                    }
                }
            }
        };
    }

    public Parser<TInput, TValue> Pre<TValue, TOpen>(Parser<TInput, TValue> parser, Parser<TInput, TOpen> openParser)
    {
        return
            from _ in openParser
            from res in parser
            select res;
    }

    public Parser<TInput, TValue> Post<TValue, TClose>(Parser<TInput, TValue> parser, Parser<TInput, TClose> closeParser)
    {
        return
            from res in parser
            from __ in closeParser
            select res;
    }

    public Parser<TInput, TValue> Between<TValue, TOpen, TClose>(Parser<TInput, TValue> parser, Parser<TInput, TOpen> openParser,
        Parser<TInput, TClose> closeParser)
    {
        return this.Post(this.Pre(parser, openParser), closeParser);
    }

    public Parser<TInput, TValue> GetLazy<TValue>(Func<Parser<TInput, TValue>> getParser)
    {
        return Parser.Return(getParser);
    }
}