using Anch.GenericQueryable.Fetching;

namespace Anch.GenericQueryable.Services;

/// <summary>An <see cref="IFetchService"/> that returns the query unchanged; used for providers without eager loading, such as LINQ to Objects.</summary>
public class IgnoreFetchService : IFetchService
{
    /// <inheritdoc/>
    public IQueryable<TSource> ApplyFetch<TSource>(IQueryable<TSource> source, FetchRule<TSource> fetchRule) where TSource : class => source;
}