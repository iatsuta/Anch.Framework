namespace CommonFramework.Parsing;

public class CharParsers : CharParsers<SharedMemoryString>
{
    public override Parser<SharedMemoryString, char> AnyChar
    {
        get
        {
            return input => input.IsEmpty ? input.ToError<char>() : input.Slice(1).ToSuccess(input.Span[0]);
        }
    }

    public Parser<SharedMemoryString, Ignore> Eof
    {
        get { return input => input.IsEmpty ? input.ToSuccess(Ignore.Value) : input.ToError<Ignore>(); }
    }

    public override Parser<SharedMemoryString, SharedMemoryString> TakeText(int charCount)
    {
        return input => input.Chars.Length < charCount ? input.ToError() : input.Split(charCount);
    }


    public override Parser<SharedMemoryString, SharedMemoryString> TakeWhile(Func<char, int, bool> predicate)
    {
        return input =>
        {
            var index = 0;

            while (index < input.Length && predicate(input.Span[index], index))
            {
                index++;
            }

            return input.Split(index);
        };
    }

    public override Parser<SharedMemoryString, SharedMemoryString> TakeWhile1(Func<char, int, bool> predicate)
    {
        return input =>
        {
            var index = 0;

            if (input.IsEmpty || !predicate(input.Span[0], index))
            {
                return input.ToError();
            }

            index ++;

            while (index < input.Length && predicate(input.Span[index], index))
            {
                index++;
            }

            return input.Split(index);
        };
    }
}


public abstract class CharParsers<TInput> : Parsers<TInput>
    where TInput : IParingInput<TInput>
{
    protected virtual IReadOnlySet<char> SpaceChars { get; } = new[] { ' ' }.ToHashSet();

    protected virtual bool IsWordChar(char c, bool isBodyChar = true)
    {
        return char.IsLetter(c) || c == '_' || (isBodyChar && char.IsDigit(c));
    }

    public abstract Parser<TInput, char> AnyChar { get; }


    public virtual Parser<TInput, SharedMemoryString> Spaces => this.TakeWhile(this.SpaceChars.Contains);

    public virtual Parser<TInput, SharedMemoryString> Spaces1 => this.TakeWhile1(this.SpaceChars.Contains);


    public Parser<TInput, TValue> BetweenBrackets<TValue>(Parser<TInput, TValue> parser)
    {
        return this.BetweenBrackets(parser, '(', ')');
    }

    public Parser<TInput, TValue> BetweenBrackets<TValue>(Parser<TInput, TValue> parser, char startBracket, char endBracket)
    {
        return this.Between(parser, this.PreSpaces(this.Char(startBracket)), this.PreSpaces(this.Char(endBracket)));
    }

    public Parser<TInput, TValue> BetweenSpaces<TValue>(Parser<TInput, TValue> parser)
    {
        return this.Between(parser, this.Spaces, this.Spaces);
    }

    public Parser<TInput, TValue> PreSpaces<TValue>(Parser<TInput, TValue> parser)
    {
        return this.Pre(parser, this.Spaces);
    }

    public Parser<TInput, TValue> PostSpaces<TValue>(Parser<TInput, TValue> parser)
    {
        return this.Post(parser, this.Spaces);
    }


    public Parser<TInput, TValue[]> SepBy<TValue>(Parser<TInput, TValue> parser, char separator)
    {
        return this.SepBy(parser, this.BetweenSpaces(this.Char(separator)));
    }

    public Parser<TInput, TValue[]> SepBy1<TValue>(Parser<TInput, TValue> parser, char separator)
    {
        return this.SepBy1(parser, this.BetweenSpaces(this.Char(separator)));
    }



    public Parser<TInput, char> Char(char ch)
    {
        return from c in this.AnyChar
               where c == ch
               select c;
    }

    public Parser<TInput, char> Char(params char[] chars)
    {
        return from c in this.AnyChar
               where chars.Contains(c)
               select c;
    }

    public Parser<TInput, bool> TryChar(char ch)
    {
        var c1 = from _ in this.Char(ch)
                 select true;

        var c2 = from _ in this.Return(ch)
                 select false;

        return c1.Or(c2);
    }

    public Parser<TInput, char> CharIgnoreCase(char ch)
    {
        return from c in this.AnyChar
               where char.ToLower(c) == char.ToLower(ch)
               select c;
    }

    public Parser<TInput, SharedMemoryString> Variable => this.TakeWhile1((c, index) => this.IsWordChar(c, index != 0));

    public Parser<TInput, SharedMemoryString> Word => this.TakeWhile(c => this.IsWordChar(c));

    public Parser<TInput, SharedMemoryString> Word1 => this.TakeWhile1(c => this.IsWordChar(c));


    public Parser<TInput, SharedMemoryString> Digits => this.TakeWhile(char.IsDigit);

    public Parser<TInput, SharedMemoryString> Digits1 => this.TakeWhile1(char.IsDigit);

    public Parser<TInput, char> Char(Func<char, bool> predicate)
    {
        return this.AnyChar.Where(predicate);
    }

    //public Parser<TInput, SharedMemoryString> String(string pattern)
    //{
    //    if (pattern == null) throw new ArgumentNullException(nameof(pattern));

    //    return from str in this.TakeText(pattern.Length)

    //           where str == pattern

    //           select str;
    //}


    //public Parser<TInput, SharedMemoryString> StringIgnoreCase(string pattern)
    //{
    //    if (pattern == null) throw new ArgumentNullException(nameof(pattern));

    //    return from str in this.TakeText(pattern.Length)

    //           where str.Equals(pattern, StringComparison.CurrentCultureIgnoreCase)

    //           select str;
    //}

    //public virtual Parser<TInput, SharedMemoryString> TakeTo(string endToken)
    //{
    //    if (endToken == null) throw new ArgumentNullException(nameof(endToken));

    //    var successParser = from _ in this.StringIgnoreCase(endToken)

    //                        select "";

    //    var nextParser = from c in this.AnyChar

    //                     from next in this.TakeTo(endToken)

    //                     select c + next;

    //    return successParser.Or(nextParser);
    //}



    //public Parser<TInput, SharedMemoryString> TakeInBracket(string startBracket, string endBracket)
    //{
    //    if (startBracket == null) throw new ArgumentNullException(nameof(startBracket));
    //    if (endBracket == null) throw new ArgumentNullException(nameof(endBracket));

    //    return from _ in this.StringIgnoreCase(startBracket)

    //           from result in this.TakeTo(endBracket)

    //           select result;
    //}


    public abstract Parser<TInput, SharedMemoryString> TakeWhile(Func<char, int, bool> predicate);

    public abstract Parser<TInput, SharedMemoryString> TakeWhile1(Func<char, int, bool> predicate);

    public Parser<TInput, SharedMemoryString> TakeWhile(Func<char, bool> predicate) =>
        this.TakeWhile((c, _) => predicate(c));

    public Parser<TInput, SharedMemoryString> TakeWhile1(Func<char, bool> predicate) =>
        this.TakeWhile1((c, _) => predicate(c));

    public Parser<TInput, SharedMemoryString> TakeLine()
    {
        return from v in this.TakeWhile(c => c != '\r' && c != '\n')

               from _ in this.TryEndLine()

               select v;
    }

    public Parser<TInput, bool> TryEndLine()
    {
        return from v1 in this.TryChar('\r')
               from v2 in this.TryChar('\n')

               select v1 || v2;
    }

    public abstract Parser<TInput, SharedMemoryString> TakeText(int charCount);


    public Parser<TInput, Guid> GuidParser
    {
        get
        {
            var guidParser = from text in this.TakeText(36)

                             from result in this.CatchParser(() => Guid.Parse(text))

                             select result;


            var withBrackets = this.Between(guidParser, this.Char('{'), this.Char('}'));


            return withBrackets.Or(guidParser);
        }
    }


    protected Parser<TInput, TResult> GetSignDigitsParser<TResult>(Func<string, TResult> parseFunc)
    {
        if (parseFunc == null) throw new ArgumentNullException(nameof(parseFunc));

        return from isNegate in this.TryChar('-')
               from value in this.Digits1
               from result in this.CatchParser(() => parseFunc((isNegate ? "-" : string.Empty) + value))
               select result;
    }

    public Parser<TInput, short> Int16Parser
    {
        get { return this.GetSignDigitsParser(short.Parse); }
    }

    public Parser<TInput, int> Int32Parser
    {
        get { return this.GetSignDigitsParser(int.Parse); }
    }

    public Parser<TInput, long> Int64Parser
    {
        get { return this.GetSignDigitsParser(long.Parse); }
    }
}
