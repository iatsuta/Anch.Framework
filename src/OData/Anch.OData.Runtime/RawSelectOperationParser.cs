using System.Globalization;
using Anch.Caching;
using Anch.Core;
using Anch.OData.Domain;
using Anch.OData.Domain.QueryLanguage;
using Anch.OData.InternalParser;
using Anch.Parsing;

namespace Anch.OData;

public class RawSelectOperationParser(ICacheProvider cacheProvider, IParsingExceptionFactory errorParsingHandler, ICultureSource? cultureSource = null)
    : IRawSelectOperationParser
{
    private readonly ICache<string, SelectOperation> cache = cacheProvider.GetCache<string, SelectOperation>(typeof(IRawSelectOperationParser));

    private readonly RawSelectOperationParserParser rawParser = new(cultureSource?.Culture ?? CultureInfo.CurrentCulture, ParameterExpression.Default);

    public SelectOperation Parse(string input) =>

        this.cache.GetOrAdd(input, _ => this.rawParser.MainParser.Parse(input, unparsedRest => errorParsingHandler.GetError(input, unparsedRest)));
}