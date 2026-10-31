using System.Collections.Concurrent;
using System.Linq.Expressions;

using Anch.Core;

namespace Anch.VisualIdentitySource;

/// <summary>Finds the name property of a type: an explicitly registered description wins, otherwise the property found by the extractor is used. Results are cached.</summary>
/// <param name="propertyExtractor">Locates the name property by convention.</param>
/// <param name="customInfoList">The explicitly registered descriptions.</param>
public class VisualIdentityInfoSource(IVisualIdentityPropertyExtractor propertyExtractor, IEnumerable<VisualIdentityInfo> customInfoList)
    : IVisualIdentityInfoSource
{
    private readonly ConcurrentDictionary<Type, VisualIdentityInfo?> cache = [];

    /// <inheritdoc/>
    public VisualIdentityInfo<TDomainObject>? TryGetVisualIdentityInfo<TDomainObject>()
    {
        return (VisualIdentityInfo<TDomainObject>?)this.TryGetVisualIdentityInfo(typeof(TDomainObject));
    }

    /// <inheritdoc/>
    public VisualIdentityInfo GetVisualIdentityInfo(Type domainObjectType)
    {
        return this.TryGetVisualIdentityInfo(domainObjectType) ?? throw GetMissedError(domainObjectType);
    }

    /// <inheritdoc/>
    public VisualIdentityInfo? TryGetVisualIdentityInfo(Type domainObjectType)
    {
        return this.cache.GetOrAdd(domainObjectType, _ =>
        {
            var customInfo = customInfoList.SingleOrDefault(identityInfo => identityInfo.DomainObjectType == domainObjectType);

            if (customInfo is not null)
            {
                return customInfo;
            }
            else
            {
                var property = propertyExtractor.TryExtract(domainObjectType);

                if (property is null)
                {
                    return null;
                }
                else
                {
                    var path = property.ToGetLambdaExpression();

                    return new Func<Expression<Func<object, string>>, VisualIdentityInfo<object>>(CreateVisualIdentityInfo)
                        .CreateGenericMethod(domainObjectType)
                        .Invoke<VisualIdentityInfo>(null, path);
                }
            }
        });
    }

    /// <inheritdoc/>
    public VisualIdentityInfo<TDomainObject> GetVisualIdentityInfo<TDomainObject>()
    {
        return this.TryGetVisualIdentityInfo<TDomainObject>() ?? throw GetMissedError(typeof(TDomainObject));
    }

    private static Exception GetMissedError(Type domainObjectType)
    {
        return new Exception($"{nameof(VisualIdentityInfo)} for {domainObjectType.Name} not found");
    }

    private static VisualIdentityInfo<TDomainObject> CreateVisualIdentityInfo<TDomainObject>(Expression<Func<TDomainObject, string>> path)
    {
        return new VisualIdentityInfo<TDomainObject>(path);
    }
}