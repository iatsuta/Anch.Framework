using System.Collections.Concurrent;

using Anch.Caching;
using Anch.Core;
using Anch.OData.Domain;

namespace Anch.OData;

/// <summary>Parses a query string into a typed operation: the raw parser builds the untyped tree and the converter binds it to the domain type. Results are cached per type and query string.</summary>
/// <param name="cacheProvider">Supplies the caches of parsed queries.</param>
/// <param name="rawParser">Parses the query string.</param>
/// <param name="selectOperationConverter">Binds the result to the domain type.</param>
public class SelectOperationParser(ICacheProvider cacheProvider, IRawSelectOperationParser rawParser, ISelectOperationConverter selectOperationConverter)
    : ISelectOperationParser
{
    private readonly ConcurrentDictionary<Type, ICache> rootCache = [];

    /// <inheritdoc/>
    public SelectOperation<TDomainObject> Parse<TDomainObject>(string input) =>
        this.rootCache
            .GetOrAddAs(typeof(TDomainObject),
                _ => cacheProvider.GetCache<string, SelectOperation<TDomainObject>>((typeof(SelectOperationParser), typeof(TDomainObject))))
            .GetOrAdd(input, _ => selectOperationConverter.Convert<TDomainObject>(rawParser.Parse(input)));
}