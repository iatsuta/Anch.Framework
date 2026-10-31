using System.Globalization;

using Anch.Caching;
using Anch.Core;
using Anch.OData.Domain;
using Anch.OData.Domain.QueryLanguage;
using Anch.OData.InternalParser;
using Anch.Parsing;

namespace Anch.OData;

/// <summary>Parses an OData query string into an untyped <see cref="SelectOperation"/>. Results are cached by query string.</summary>
/// <param name="cacheProvider">Supplies the cache of parsed queries.</param>
/// <param name="errorParsingHandler">Creates the exception for an invalid query.</param>
/// <param name="cultureSource">The culture used to parse numbers and dates; the invariant culture when <c>null</c>.</param>
public class RawSelectOperationParser(ICacheProvider cacheProvider, IParsingExceptionFactory errorParsingHandler, ICultureSource? cultureSource = null)
    : IRawSelectOperationParser
{
    private readonly ICache<string, SelectOperation> cache = cacheProvider.GetCache<string, SelectOperation>(typeof(IRawSelectOperationParser));

    private readonly RawSelectOperationParserParser rawParser = new(cultureSource?.Culture ?? CultureInfo.InvariantCulture, ParameterExpression.Default);

    /// <inheritdoc/>
    public SelectOperation Parse(string input) =>

        this.cache.GetOrAdd(input, _ => this.rawParser.MainParser.Parse(input, unparsedRest => errorParsingHandler.GetError(input, unparsedRest)));
}