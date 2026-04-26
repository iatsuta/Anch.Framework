using System.Collections.Concurrent;
using Anch.Caching;
using Anch.Core;
using Anch.OData.Domain;

namespace Anch.OData;

public class SelectOperationParser(ICacheProvider cacheProvider, IRawSelectOperationParser rawParser, ISelectOperationConverter selectOperationConverter)
    : ISelectOperationParser
{
    private readonly ConcurrentDictionary<Type, ICache> rootCache = [];

    public SelectOperation<TDomainObject> Parse<TDomainObject>(string input) =>
        this.rootCache
            .GetOrAddAs(typeof(TDomainObject),
                _ => cacheProvider.GetCache<string, SelectOperation<TDomainObject>>((typeof(SelectOperationParser), typeof(TDomainObject))))
            .GetOrAdd(input, _ => selectOperationConverter.Convert<TDomainObject>(rawParser.Parse(input)));
}