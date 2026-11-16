namespace Anch.VisualIdentitySource;

/// <summary>Formats objects with the display function found by the display info source.</summary>
/// <param name="displayObjectInfoSource">Finds the display function of a type.</param>
public class DomainObjectDisplayService(IDisplayObjectInfoSource displayObjectInfoSource) : IDomainObjectDisplayService
{
    /// <inheritdoc/>
    public string Format<TDomainObject>(TDomainObject domainObject)
        where TDomainObject : class =>
        displayObjectInfoSource.GetDisplayObjectInfo<TDomainObject>().DisplayFunc(domainObject);
}