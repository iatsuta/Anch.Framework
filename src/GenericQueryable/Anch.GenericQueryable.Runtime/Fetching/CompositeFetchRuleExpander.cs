using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.Fetching;

/// <summary>Expands a <see cref="CompositeFetchRule{TSource}"/> by expanding both parts and joining their paths.</summary>
/// <param name="serviceProvider">Supplies the root expander used for the parts.</param>
public class CompositeFetchRuleExpander(IServiceProvider serviceProvider) : IFetchRuleExpander
{
    private IFetchRuleExpander RootFetchRuleExpander => field ??= serviceProvider.GetRequiredService<IFetchRuleExpander>();

    /// <inheritdoc/>
    public PropertyFetchRule<TSource>? TryExpand<TSource>(FetchRule<TSource> fetchRule)
    {
        if (fetchRule is CompositeFetchRule<TSource> compositeFetchRule
            && this.RootFetchRuleExpander.TryExpand(compositeFetchRule.Left) is { } left
            && this.RootFetchRuleExpander.TryExpand(compositeFetchRule.Right) is { } right)
        {
            return new PropertyFetchRule<TSource>(left.Paths.Concat(right.Paths));
        }
        else
        {
            return null;
        }
    }
}