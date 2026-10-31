using System.Collections.Concurrent;

using Anch.Core;

namespace Anch.VisualIdentitySource;

/// <summary>Finds how a type is displayed: an explicitly registered function wins, then the name property, then <c>ToString()</c>. Results are cached.</summary>
/// <param name="visualIdentityInfoSource">Finds the name property.</param>
/// <param name="customDisplayObjectInfoList">The explicitly registered display functions.</param>
public class DisplayObjectInfoSource(IVisualIdentityInfoSource visualIdentityInfoSource, IEnumerable<DisplayObjectInfo> customDisplayObjectInfoList) : IDisplayObjectInfoSource
{
    private readonly ConcurrentDictionary<Type, DisplayObjectInfo> cache = [];

    /// <inheritdoc/>
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