using Anch.GenericQueryable.Fetching;

namespace Anch.GenericQueryable;

/// <summary>
/// Eager loading (<c>WithFetch</c>) for <see cref="IQueryable{T}"/>, independent of the ORM.
/// </summary>
/// <example>
/// <code>
/// query.WithFetch(r =&gt; r.Fetch(x =&gt; x.Orders).ThenFetch(o =&gt; o.Items));
/// query.WithFetch("Orders.Items");
/// </code>
/// </example>
public static class GenericQueryableFetchExtensions
{
    extension<TSource>(IQueryable<TSource> source)
        where TSource : class
    {
        /// <summary>Loads the related data described by a dotted property path (an <see cref="UntypedFetchRule{TSource}"/>).</summary>
        /// <param name="fetchPath">Dotted property path, for example <c>"Orders.Items"</c>.</param>
        public IQueryable<TSource> WithFetch(string fetchPath) => source.WithFetch(new UntypedFetchRule<TSource>(fetchPath));

        /// <summary>Loads the related data described by a fluent fetch rule.</summary>
        /// <param name="buildFetchRule">Builds the rule starting from <see cref="FetchRule{TSource}.Empty"/>, using <c>Fetch</c> and <c>ThenFetch</c>.</param>
        public IQueryable<TSource> WithFetch(Func<PropertyFetchRule<TSource>, FetchRule<TSource>> buildFetchRule) =>
            source.WithFetch(buildFetchRule.ToFetchRule());

        /// <summary>
        /// Applies <paramref name="fetchRule"/> through the <see cref="IFetchService"/> of the queryable's provider.
        /// For a provider without fetch support the rule is ignored.
        /// </summary>
        /// <param name="fetchRule">The fetch rule to apply.</param>
        public IQueryable<TSource> WithFetch(FetchRule<TSource> fetchRule) => source.Execute(executor => executor.FetchService.ApplyFetch(source, fetchRule));
    }
}