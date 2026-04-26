using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using Anch.Core;

namespace Anch.GenericQueryable.Fetching;

public class FetchRuleHeaderExpander(IEnumerable<FetchRuleHeaderInfo> fetchRuleHeaderInfoList) : IFetchRuleExpander
{
    private readonly FrozenDictionary<Type, ImmutableArray<FetchRuleHeaderInfo>> headersDict =
        fetchRuleHeaderInfoList.GroupBy(v => v.SourceType).ToFrozenDictionary(g => g.Key, ImmutableArray<FetchRuleHeaderInfo> (g) => [..g]);

    private readonly ConcurrentDictionary<Type, object> cache = [];

    public PropertyFetchRule<TSource>? TryExpand<TSource>(FetchRule<TSource> fetchRule)
    {
        if (fetchRule is FetchRuleHeader<TSource> fetchRuleHeader)
        {
            return this.cache
                .GetOrAddAs(fetchRuleHeader.GetType(), _ => this.headersDict
                    .GetValueOrDefault(typeof(TSource))
                    .EmptyIfNull()
                    .Cast<FetchRuleHeaderInfo<TSource>>()
                    .ToFrozenDictionary(info => info.Header, info => info.Implementation))
                .GetValueOrDefault(fetchRuleHeader);
        }
        else
        {
            return null;
        }
    }
}