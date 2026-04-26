namespace Anch.Core;

public static class StringExtensions
{
    extension(string input)
    {
        public string FromPluralize()
        {
            if (input.Length < 2)
            {
                return input;
            }
            else if (input.EndsWith("ies", StringComparison.Ordinal) && input.Length > 3)
            {
                return input[..^3] + "y";
            }
            else if (input.EndsWith("es", StringComparison.Ordinal))
            {
                if (input.Length > 4 && input[^4] != 's')
                {
                    return input[..^2];
                }
                else
                {
                    return input[..^1];
                }
            }
            else if (input.EndsWith('s') && !input.EndsWith("ss", StringComparison.Ordinal))
            {
                return input[..^1];
            }
            else
            {
                return input;
            }
        }


        public string ToStartLowerCase() => input.Length != 0 ? char.ToLower(input.First()) + input[1..] : input;

        public string Skip(string pattern, bool raiseIfNotEquals) => input.Skip(pattern, StringComparison.CurrentCulture, raiseIfNotEquals);

        public string Skip(string pattern, StringComparison stringComparison, bool raiseIfNotEquals)
        {
            if (input.StartsWith(pattern, stringComparison))
            {
                return input[pattern.Length..];
            }
            else if (raiseIfNotEquals)
            {
                throw new ArgumentException($"Invalid input: {input}. Expected start element: {pattern}", nameof(input));
            }
            else
            {
                return input;
            }
        }

        public string SkipLast(string pattern, bool raiseIfNotEquals)
        {
            return input.SkipLast(pattern, StringComparison.CurrentCulture, raiseIfNotEquals);
        }

        public string SkipLast(string pattern, StringComparison stringComparison, bool raiseIfNotEquals)
        {
            if (input.EndsWith(pattern, stringComparison))
            {
                return input[..^pattern.Length];
            }
            else if (raiseIfNotEquals)
            {
                throw new ArgumentException($"Invalid input: {input}. Expected last element: {pattern}", nameof(input));
            }
            else
            {
                return input;
            }
        }
    }

    extension<TSource>(IEnumerable<TSource> source)
    {
        public string Join<TResult>(string separator, Func<TSource, TResult> selector)
        {
            return source.Select(selector).Join(separator);
        }

        public string Join(string separator)
        {
            return string.Join(separator, source);
        }
    }
}