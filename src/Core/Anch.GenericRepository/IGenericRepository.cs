namespace Anch.GenericRepository;

/// <summary>Saves and removes entities. The application implements it on top of its ORM; framework modules depend only on this interface.</summary>
public interface IGenericRepository
{
    /// <summary>Saves a new or changed entity.</summary>
    /// <param name="data">The entity.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <typeparam name="TDomainObject">The entity type.</typeparam>
    Task SaveAsync<TDomainObject>(TDomainObject data, CancellationToken ct)
        where TDomainObject : class;

    /// <summary>Removes an entity.</summary>
    /// <param name="data">The entity.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <typeparam name="TDomainObject">The entity type.</typeparam>
    Task RemoveAsync<TDomainObject>(TDomainObject data, CancellationToken ct)
        where TDomainObject : class;
}