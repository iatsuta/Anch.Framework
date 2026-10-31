namespace Anch.GenericQueryable.Fetching;

/// <summary>ORM-specific application of a fetch rule to a query (for example <c>Include</c> in EF Core, <c>Fetch</c> in NHibernate).</summary>
public interface IFetchService
{
    /// <summary>Returns the query extended with eager loading described by <paramref name="fetchRule"/>.</summary>
    /// <param name="source">The query to extend.</param>
    /// <param name="fetchRule">The rule to apply.</param>
    /// <typeparam name="TSource">The root entity type.</typeparam>
    IQueryable<TSource> ApplyFetch<TSource>(IQueryable<TSource> source, FetchRule<TSource> fetchRule)
        where TSource : class;
}