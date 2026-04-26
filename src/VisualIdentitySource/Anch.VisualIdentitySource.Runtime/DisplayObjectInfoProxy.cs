namespace Anch.VisualIdentitySource;

public class DisplayObjectInfoProxy<TDomainObject>(IDisplayObjectInfoSource displayObjectInfoSource) : IDisplayObjectInfo<TDomainObject>
    where TDomainObject : class
{
    public Func<TDomainObject, string> DisplayFunc { get; } = displayObjectInfoSource.GetDisplayObjectInfo<TDomainObject>().DisplayFunc;
}