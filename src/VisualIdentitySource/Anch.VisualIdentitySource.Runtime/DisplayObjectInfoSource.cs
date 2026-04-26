using System.Collections.Concurrent;

using Anch.Core;

namespace Anch.VisualIdentitySource;

public class DisplayObjectInfoSource(IVisualIdentityInfoSource visualIdentityInfoSource, IEnumerable<DisplayObjectInfo> customDisplayObjectInfoList) : IDisplayObjectInfoSource
{
    private readonly ConcurrentDictionary<Type, DisplayObjectInfo> cache = [];

    public DisplayObjectInfo<TDomainObject> GetDisplayObjectInfo<TDomainObject>()
        where TDomainObject : class
    {
        return this.cache.GetOrAddAs(typeof(TDomainObject), _ =>
        {
            if (customDisplayObjectInfoList.SingleOrDefault(info => info.DomainObjectType == typeof(TDomainObject)) is { } customInfo)
            {
                return (DisplayObjectInfo<TDomainObject>)customInfo;
            }
            else if (visualIdentityInfoSource.TryGetVisualIdentityInfo<TDomainObject>() is { } visualIdentityInfo)
            {
                return new DisplayObjectInfo<TDomainObject>(visualIdentityInfo.Name.Getter);
            }
            else
            {
                return DisplayObjectInfo<TDomainObject>.Default;
            }
        });
    }
}