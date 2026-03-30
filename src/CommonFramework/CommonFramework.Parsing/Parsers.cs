namespace CommonFramework.Parsing;

public abstract class Parsers<TInput>
    where TInput : IParingInput<TInput>
{
    protected Parser<TInput, TValue> CatchParser<TValue>(Func<TValue> createFunc)
    {
        if (createFunc == null) throw new ArgumentNullException(nameof(createFunc));

        try   { return this.Return(createFunc()); }
        catch { return this.Fault<TValue>(); }
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

    public Parser<TInput, Func<TIdentity, TIdentity>> GetIdentity<TIdentity>()
    {
        return this.Return(new Func<TIdentity, TIdentity>(v => v));
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


    public Parser<TInput, TValue[]> Many<TValue>(Parser<TInput, TValue> parser)
    {
        return this.Many1(parser).Or(this.Return(Array.Empty<TValue>()));
    }

    public Parser<TInput, TValue[]> Many1<TValue>(Parser<TInput, TValue> parser)
    {
        return input =>
        {
            var prevResult = parser(input);

            if (prevResult.HasError)
            {
                return prevResult.ToError<TValue[]>();
            }
            else
            {
                for (var list = new List<TValue> { prevResult.Value };; list.Add(prevResult.Value))
                {
                    var nextResult = parser(prevResult.Rest);

                    if (nextResult.HasError)
                    {
                        return new ParsingState<TInput, TValue[]>(list.ToArray(), prevResult.Rest);
                    }
                    else
                    {
                        prevResult = nextResult;
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


    public Parser<TInput, TValue[]> SepBy1<TValue, TSeparator>(Parser<TInput, TValue> parser, Parser<TInput, TSeparator> separatorParser)
    {
        return from x in parser

               from xs in this.Many(this.Pre(parser, separatorParser))

               select (TValue[])[x, .. xs];
    }

    public Parser<TInput, TValue[]> SepBy<TValue, TSeparator>(Parser<TInput, TValue> parser, Parser<TInput, TSeparator> separatorParser)
    {
        return this.SepBy1(parser, separatorParser).Or(this.Return(Array.Empty<TValue>()));
    }

    public Parser<TInput, TValue> Pre<TValue, TOpen>(Parser<TInput, TValue> parser, Parser<TInput, TOpen> openParser)
    {
        return from _ in openParser
               from res in parser
               select res;
    }

    public Parser<TInput, TValue> Post<TValue, TClose>(Parser<TInput, TValue> parser, Parser<TInput, TClose> closeParser)
    {
        return from res in parser
               from __ in closeParser
               select res;
    }

    public Parser<TInput, TValue> Between<TValue, TOpen, TClose> (Parser<TInput, TValue> parser, Parser<TInput, TOpen> openParser, Parser<TInput, TClose> closeParser)
    {
        return this.Post(this.Pre (parser, openParser), closeParser);
    }


    public Parser<TInput, TValue> GetLazy<TValue>(Func<Parser<TInput, TValue>> getParser)
    {
        return input => getParser()(input);
    }
}

public static class Parser<TInput>
{
    public static Parser<TInput, TValue> Return<TValue>(TValue value)
    {
        return input => new ParsingState<TInput, TValue>(value, input);
    }
}
