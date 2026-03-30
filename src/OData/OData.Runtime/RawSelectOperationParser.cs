using CommonFramework;
using CommonFramework.Parsing;

using OData.Domain;
using OData.Domain.QueryLanguage;
using OData.InternalParser;

using System.Collections.Concurrent;
using System.Globalization;

namespace OData;

public class RawSelectOperationParser(ICultureSource? cultureSource = null) : IRawSelectOperationParser
{
    private readonly RawSelectOperationParserParser rawParser = new(cultureSource?.Culture ?? CultureInfo.CurrentCulture, ParameterExpression.Default);

    private readonly ConcurrentDictionary<string, SelectOperation> cache = [];

    public SelectOperation Parse(string input) =>

        this.cache.GetOrAdd(input, _ => rawParser.MainParser.Parse(input, unparsedRest => new ODataParsingException($"Can't parse: {unparsedRest}")));
}