using System.Globalization;

namespace CommonFramework.Parsing;

public class DecimalParser(CultureInfo culture) : CharParsers(culture)
{
    private readonly string separator = culture.NumberFormat.NumberDecimalSeparator;

    public Parser<SharedMemoryString, decimal> Decimal =>

        input =>
        {
            if (!input.IsEmpty)
            {
                var span = input.Span;

                var index = 0;

                if (span[index] == '-')
                {
                    index++;
                }

                if (ReadNumber(input, ref index))
                {
                    var sepIndex = 0;

                    for (; index < span.Length && sepIndex < this.separator.Length; sepIndex++)
                    {
                        if (span[index] == this.separator[sepIndex])
                        {
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (sepIndex == 0 || (sepIndex == this.separator.Length && ReadNumber(input, ref index)))
                    {
                        var parts = input.Split(index);

                        if (decimal.TryParse(parts.Item1, culture, out var result))
                        {
                            return new ParsingState<SharedMemoryString, decimal>(result, parts.Item2);
                        }
                    }
                }
            }

            return input.ToError<decimal>();
        };

    private static bool ReadNumber(SharedMemoryString input, ref int index)
    {
        var span = input.Span;

        if (index < span.Length && char.IsDigit(span[index]))
        {
            index++;
        }
        else
        {
            return false;
        }

        while (index < span.Length && char.IsDigit(span[index]))
        {
            index++;
        }

        return true;
    }
}