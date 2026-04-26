namespace Anch.Parsing;

public static class ParserExtensions
{
    extension<TInput, TValue>(Parser<TInput, TValue> parser)
    {
        public TValue Parse(TInput value) =>
            parser.Parse(value, unparsedRest => new Exception($"Can't parse: {unparsedRest}"));

        public TValue Parse(TInput value, Func<TInput, Exception> getUnparsedRestException)
        {
            var parsingResult = parser(value);

            if (parsingResult.HasError)
            {
                throw getUnparsedRestException(parsingResult.Rest);
            }
            else
            {
                return parsingResult.Value;
            }
        }
    }
}