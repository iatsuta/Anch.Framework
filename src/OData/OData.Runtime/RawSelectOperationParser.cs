using CommonFramework;
using CommonFramework.Caching;
using CommonFramework.Parsing;

using OData.Domain;
using OData.Domain.QueryLanguage;
using OData.InternalParser;

using System.Globalization;

namespace OData;

public class RawSelectOperationParser(ICacheProvider cacheProvider, IParsingExceptionFactory errorParsingHandler, ICultureSource? cultureSource = null)
    : IRawSelectOperationParser
{
    private readonly ICache<string, SelectOperation> cache = cacheProvider.GetCache<string, SelectOperation>(typeof(IRawSelectOperationParser));

    private readonly RawSelectOperationParserParser rawParser = new(cultureSource?.Culture ?? CultureInfo.CurrentCulture, ParameterExpression.Default);

    public SelectOperation Parse(string input) =>

        this.cache.GetOrAdd(input, _ => this.rawParser.MainParser.Parse(input, unparsedRest => errorParsingHandler.GetError(input, unparsedRest)));
}