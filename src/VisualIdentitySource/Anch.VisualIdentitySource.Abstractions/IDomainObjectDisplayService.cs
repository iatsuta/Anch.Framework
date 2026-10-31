namespace Anch.VisualIdentitySource;

/// <summary>Formats any domain object as text for messages and user interfaces.</summary>
public interface IDomainObjectDisplayService
{
    /// <summary>Returns the display text of an object.</summary>
    /// <param name="domainObject">The object to format.</param>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    string Format<TDomainObject>(TDomainObject domainObject)
        where TDomainObject : class;
}