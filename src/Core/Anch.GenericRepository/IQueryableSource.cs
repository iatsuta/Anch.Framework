namespace Anch.GenericRepository;

/// <summary>Provides queries over entities. The application implements it on top of its ORM; framework modules depend only on this interface.</summary>
public interface IQueryableSource
{
    /// <summary>Returns a query over all entities of a type.</summary>
    /// <typeparam name="TDomainObject">The entity type.</typeparam>
    IQueryable<TDomainObject> GetQueryable<TDomainObject>()
        where TDomainObject : class;
}