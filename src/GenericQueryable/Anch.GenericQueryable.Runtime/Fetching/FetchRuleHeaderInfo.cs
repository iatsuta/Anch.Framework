namespace Anch.GenericQueryable.Fetching;

/// <summary>A registered named fetch rule.</summary>
public abstract record FetchRuleHeaderInfo
{
    /// <summary>The root entity type of the rule.</summary>
    public abstract Type SourceType { get; }
}

/// <summary>Binds a named fetch rule to the property paths it stands for.</summary>
/// <param name="Header">The named rule.</param>
/// <param name="Implementation">The paths to load.</param>
/// <typeparam name="TSource">The root entity type.</typeparam>
public record FetchRuleHeaderInfo<TSource>(FetchRuleHeader<TSource> Header, PropertyFetchRule<TSource> Implementation) : FetchRuleHeaderInfo
{
    /// <inheritdoc/>
    public override Type SourceType { get; } = typeof(TSource);
}