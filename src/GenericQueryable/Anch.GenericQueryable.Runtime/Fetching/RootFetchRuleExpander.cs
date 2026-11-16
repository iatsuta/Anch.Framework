using System.Collections.Concurrent;

using Anch.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.Fetching;

/// <summary>The expander that the fetch service uses: returns property rules as they are and asks the registered expanders for any other rule. Results are cached.</summary>
/// <param name="expanders">The registered expanders, tried in order.</param>
public class RootFetchRuleExpander([FromKeyedServices(IFetchRuleExpander.ElementKey)] IEnumerable<IFetchRuleExpander> expanders) : IFetchRuleExpander
{
    private readonly ConcurrentDictionary<Type, object> cache = [];

    /// <inheritdoc/>
    public PropertyFetchRule<TSource>? TryExpand<TSource>(FetchRule<TSource> fetchRule)
    {
        if (fetchRule is PropertyFetchRule<TSource> propertyFetchRule)
        {
            return propertyFetchRule;
        }
        else
        {
            return this.cache
                .GetOrAddAs(fetchRule.GetType(), _ => new ConcurrentDictionary<FetchRule<TSource>, PropertyFetchRule<TSource>?>())
                .GetOrAdd(fetchRule, _ =>
                {
                    var request =

                        from expander in expanders

                        let expandedFetchRule = expander.TryExpand(fetchRule)

                        where expandedFetchRule is not null

                        select expandedFetchRule;

                    return request.FirstOrDefault();
                });
        }
    }
}