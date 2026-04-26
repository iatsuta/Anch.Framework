namespace Anch.VisualIdentitySource;

public class DomainObjectDisplayService(IDisplayObjectInfoSource displayObjectInfoSource) : IDomainObjectDisplayService
{
    public string Format<TDomainObject>(TDomainObject domainObject)
        where TDomainObject : class =>
        displayObjectInfoSource.GetDisplayObjectInfo<TDomainObject>().DisplayFunc(domainObject);
}