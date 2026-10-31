using Anch.GenericQueryable.Fetching;
using Anch.GenericQueryable.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.DependencyInjection;

/// <summary>Fluent configuration of GenericQueryable.</summary>
public interface IGenericQueryableSetup : IGenericQueryableSetup<IGenericQueryableSetup>
{
    /// <summary>Sets the service that applies fetch rules; ORM adapters supply their own.</summary>
    /// <typeparam name="TFetchService">The service type.</typeparam>
    IGenericQueryableSetup SetFetchService<TFetchService>()
        where TFetchService : IFetchService;

    /// <summary>Adds an expander that turns custom fetch rules into property paths.</summary>
    /// <typeparam name="TFetchRuleExpander">The expander type.</typeparam>
    IGenericQueryableSetup AddFetchRuleExpander<TFetchRuleExpander>()
        where TFetchRuleExpander : IFetchRuleExpander;

    /// <summary>Registers the property paths that a named fetch rule stands for.</summary>
    /// <param name="header">The named rule.</param>
    /// <param name="implementation">The paths to load.</param>
    /// <typeparam name="TSource">The root entity type.</typeparam>
    IGenericQueryableSetup AddFetchRule<TSource>(FetchRuleHeader<TSource> header, PropertyFetchRule<TSource> implementation);

    /// <summary>Sets how <c>GenericXxxAsync</c> methods are mapped to the ORM's methods; ORM adapters supply their own.</summary>
    /// <typeparam name="TTargetMethodExtractor">The extractor type.</typeparam>
    IGenericQueryableSetup SetTargetMethodExtractor<TTargetMethodExtractor>()
        where TTargetMethodExtractor : ITargetMethodExtractor;
}

/// <summary>The part of the configuration shared with the ORM adapters.</summary>
/// <typeparam name="TSelf">The concrete setup type, for fluent chaining.</typeparam>
public interface IGenericQueryableSetup<out TSelf>
    where TSelf : IGenericQueryableSetup<TSelf>
{
    /// <summary>Adds an extension that registers additional services.</summary>
    /// <param name="extension">The extension.</param>
    TSelf AddExtension(IGenericQueryableExtension extension);

    /// <summary>Adds an extension created with its parameterless constructor.</summary>
    /// <typeparam name="TGenericQueryableExtension">The extension type.</typeparam>
    TSelf AddExtension<TGenericQueryableExtension>()
        where TGenericQueryableExtension : IGenericQueryableExtension, new() =>
        this.AddExtension(new TGenericQueryableExtension());

    /// <summary>Registers additional services together with GenericQueryable.</summary>
    /// <param name="setupAction">Registers the services.</param>
    TSelf AddServices(Action<IServiceCollection> setupAction) => this.AddExtension(new GenericQueryableExtension(setupAction));
}