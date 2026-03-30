using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace CommonFramework.VisualIdentitySource;

public class VisualIdentityInfoSource(IVisualIdentityPropertyExtractor propertyExtractor, IEnumerable<VisualIdentityInfo> customInfoList)
	: IVisualIdentityInfoSource
{
    private readonly ConcurrentDictionary<Type, VisualIdentityInfo?> cache = [];

    public VisualIdentityInfo<TDomainObject>? TryGetVisualIdentityInfo<TDomainObject>()
    {
        return (VisualIdentityInfo<TDomainObject>?)this.TryGetVisualIdentityInfo(typeof(TDomainObject));
    }

    public VisualIdentityInfo GetVisualIdentityInfo(Type domainObjectType)
    {
        return this.TryGetVisualIdentityInfo(domainObjectType) ?? throw GetMissedError(domainObjectType);
    }

    public VisualIdentityInfo? TryGetVisualIdentityInfo(Type domainObjectType)
    {
        return this.cache.GetOrAdd(domainObjectType, _ =>
        {
            var customInfo = customInfoList.SingleOrDefault(identityInfo => identityInfo.DomainObjectType == domainObjectType);

            if (customInfo != null)
            {
                return customInfo;
            }
            else
            {
                var property = propertyExtractor.TryExtract(domainObjectType);

                if (property == null)
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