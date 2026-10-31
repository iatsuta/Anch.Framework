namespace Anch.Core;

/// <summary>Adds a restriction to a query: a filter, an ordering or paging. Implemented, for example, by security providers (access filter) and by parsed OData operations.</summary>
/// <typeparam name="T">The element type of the query.</typeparam>
public interface IQueryableInjector<T>
{
    /// <summary>Returns the query with the restriction applied.</summary>
    /// <param name="queryable">The query to extend.</param>
    IQueryable<T> Inject(IQueryable<T> queryable);
}