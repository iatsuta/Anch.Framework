using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace CommonFramework.IdentitySource;

public class IdentityInfoSource(IIdentityPropertyExtractor propertyExtractor, IEnumerable<IdentityInfo> customInfoList)
    : IIdentityInfoSource
{
    private readonly ConcurrentDictionary<Type, IdentityInfo?> cache = [];

    public IdentityInfo<TDomainObject>? TryGetIdentityInfo<TDomainObject>()
    {
        return (IdentityInfo<TDomainObject>?)this.TryGetIdentityInfo(typeof(TDomainObject));
    }

    public IdentityInfo GetIdentityInfo(Type domainObjectType)
    {
        return this.TryGetIdentityInfo(domainObjectType) ?? throw GetMissedError(domainObjectType);
    }

    public IdentityInfo? TryGetIdentityInfo(Type domainObjectType)
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

                    return new Func<Expression<Func<object, object>>, IdentityInfo<object, object>>(CreateIdentityInfo)
                        .CreateGenericMethod(domainObjectType, property.PropertyType)
                        .Invoke<IdentityInfo>(null, path);
                }
            }
        });
    }

    public IdentityInfo<TDomainObject> GetIdentityInfo<TDomainObject>()
    {
        return this.TryGetIdentityInfo<TDomainObject>() ?? throw GetMissedError(typeof(TDomainObject));
    }

    private static Exception GetMissedError(Type domainObjectType)
    {
        return new Exception($"{nameof(IdentityInfo)} for {domainObjectType.Name} not found");
    }

    private static IdentityInfo<TDomainObject, TIdent> CreateIdentityInfo<TDomainObject, TIdent>(Expression<Func<TDomainObject, TIdent>> idPath)
        where TIdent : notnull
    {
        return new IdentityInfo<TDomainObject, TIdent>(idPath);
    }
}