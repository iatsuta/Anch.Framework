using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using System.Numerics;

using Anch.Core;

namespace Anch.Parsing;

public class CharParsers(CultureInfo culture) : CharParsers<SharedMemoryString>(culture)
{
    public override Parser<SharedMemoryString, char> AnyChar
    {
        get
        {
            return input => input.IsEmpty ? input.ToError<char>() : input[1..].ToSuccess(input.Span[0]);
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


    public override Parser<SharedMemoryString, SharedMemoryString> TakeTo(SharedMemoryString endToken)
    {
        return input =>
        {
            var index = 0;

            while (index <= input.Length - endToken.Length)
            {
                if (input.Slice(index, endToken.Length).Equals(endToken))
                {
                    var preResult = input.Split(index);

                    return preResult with { Item2 = preResult.Item2.Split(endToken.Length).Item2 };
                }
                else
                {
                    index++;
                }
            }

            return input.ToError();
        };
    }
}


public abstract class CharParsers<TInput>(CultureInfo culture) : Parsers<TInput>
{
    protected virtual FrozenSet<char> SpaceChars { get; } = [' '];

    protected virtual bool IsWordChar(char c, bool isBodyChar = true)
    {
        return char.IsLetter(c) || c == '_' || (isBodyChar && char.IsDigit(c));
    }

    public abstract Parser<TInput, char> AnyChar { get; }


    public Parser<TInput, SharedMemoryString> Spaces => this.TakeWhile(this.SpaceChars.Contains);

    public Parser<TInput, SharedMemoryString> Spaces1 => this.TakeWhile1(this.SpaceChars.Contains);

    public Parser<TInput, TValue> BetweenBrackets<TValue>(Parser<TInput, TValue> parser) => this.BetweenBrackets(parser, '(', ')');

    public Parser<TInput, TValue> BetweenBrackets<TValue>(Parser<TInput, TValue> parser, char startBracket, char endBracket) =>
        this.Between(parser, this.PreSpaces(this.Char(startBracket)), this.PreSpaces(this.Char(endBracket)));

    public Parser<TInput, TValue> BetweenSpaces<TValue>(Parser<TInput, TValue> parser) => this.Between(parser, this.Spaces, this.Spaces);

    public Parser<TInput, TValue> PreSpaces<TValue>(Parser<TInput, TValue> parser) => this.Pre(parser, this.Spaces);

    public Parser<TInput, TValue> PostSpaces<TValue>(Parser<TInput, TValue> parser) => this.Post(parser, this.Spaces);

    public Parser<TInput, ImmutableArray<TValue>> SepBy<TValue>(Parser<TInput, TValue> parser, char separator) =>
        this.SepBy(parser, this.BetweenSpaces(this.Char(separator)));

    public Parser<TInput, ImmutableArray<TValue>> SepBy1<TValue>(Parser<TInput, TValue> parser, char separator) =>
        this.SepBy1(parser, this.BetweenSpaces(this.Char(separator)));


    public Parser<TInput, char> Char(char ch) =>

        from c in this.AnyChar

        where c == ch

        select c;

    public Parser<TInput, char> Char(params char[] chars) =>

        from c in this.AnyChar

        where chars.Contains(c)

        select c;

    public Parser<TInput, bool> TryChar(char ch) => this.Char(ch).Select(_ => true).Or(this.Return(false));

    public Parser<TInput, bool> TryString(SharedMemoryString text) => this.String(text).Select(_ => true).Or(this.Return(false));

    public Parser<TInput, char> CharIgnoreCase(char ch) =>

        from c in this.AnyChar

        where char.ToLower(c) == char.ToLower(ch)

        select c;

    public Parser<TInput, SharedMemoryString> Variable => this.TakeWhile1((c, index) => this.IsWordChar(c, index != 0));

    public Parser<TInput, SharedMemoryString> Word => this.TakeWhile(c => this.IsWordChar(c));

    public Parser<TInput, SharedMemoryString> Word1 => this.TakeWhile1(c => this.IsWordChar(c));

    public Parser<TInput, char> Digit => this.Char(char.IsDigit);

    public Parser<TInput, SharedMemoryString> Digits => this.TakeWhile(char.IsDigit);

    public Parser<TInput, SharedMemoryString> Digits1 => this.TakeWhile1(char.IsDigit);

    public Parser<TInput, char> Char(Func<char, bool> predicate) => this.AnyChar.Where(predicate);

    public Parser<TInput, SharedMemoryString> String(SharedMemoryString pattern) =>

        from str in this.TakeText(pattern.Length)

        where str == pattern

        select str;

    public Parser<TInput, SharedMemoryString> StringIgnoreCase(SharedMemoryString pattern) =>

        from str in this.TakeText(pattern.Length)

        where str.Equals(pattern, StringComparison.OrdinalIgnoreCase)

        select str;

    public Parser<TInput, SharedMemoryString> TakeInBracket(SharedMemoryString startBracket, SharedMemoryString endBracket)
    {
        return

            from _ in this.StringIgnoreCase(startBracket)

            from result in this.TakeTo(endBracket)

            select result;
    }


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

    public abstract Parser<TInput, SharedMemoryString> TakeTo(SharedMemoryString endToken);

    public Parser<TInput, Guid> GuidParser
    {
        get
        {
            var guidParser =

                from text in this.TakeText(36)

                from result in this.SpanParser<Guid>(text)

                select result;

            var withBrackets = this.Between(guidParser, this.Char('{'), this.Char('}'));


            return withBrackets.Or(guidParser);
        }
    }
    protected Parser<TInput, TValue> SpanParser<TValue>(SharedMemoryString str)
        where TValue : struct, ISpanParsable<TValue>
    {
        return this.MaybeParser(() => Maybe.OfCondition(TValue.TryParse(str, culture, out var result), () => result));
    }

    public Parser<TInput, TNumber> GetNumberParser<TNumber>()
        where TNumber : struct, IUnaryNegationOperators<TNumber, TNumber>, ISpanParsable<TNumber>
    {
        return

            from isNegate in this.TryString(culture.NumberFormat.NegativeSign)

            from digits in this.Digits1

            from preResult in this.SpanParser<TNumber>(digits)

            select isNegate ? -preResult : preResult;
    }

    public Parser<TInput, short> Int16Parser => this.GetNumberParser<short>();

    public Parser<TInput, int> Int32Parser => this.GetNumberParser<int>();

    public Parser<TInput, long> Int64Parser => this.GetNumberParser<long>();

    public Parser<TInput, bool> BooleanParser =>

        (this.StringIgnoreCase(bool.TrueString).Select(_ => true))

        .Or(() => this.StringIgnoreCase(bool.FalseString).Select(_ => false));

    public Parser<TInput, TValue> FromDictionary<TKey, TValue, TParseKeyResult>(IReadOnlyDictionary<TKey, TValue> dictionary,
        Func<TKey, Parser<TInput, TParseKeyResult>> getKeyParser)

        where TKey : notnull =>

        dictionary.Aggregate(this.Fault<TValue>(), (state, pair) => state.Or(() => from _ in getKeyParser(pair.Key)
            select pair.Value));
}