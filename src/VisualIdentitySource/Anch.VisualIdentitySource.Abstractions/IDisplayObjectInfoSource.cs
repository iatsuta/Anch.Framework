namespace Anch.VisualIdentitySource;

public interface IDisplayObjectInfoSource
{
    DisplayObjectInfo<TDomainObject> GetDisplayObjectInfo<TDomainObject>()
        where TDomainObject : class;
}