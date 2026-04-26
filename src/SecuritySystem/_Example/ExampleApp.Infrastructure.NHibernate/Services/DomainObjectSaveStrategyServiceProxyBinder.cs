using Anch.Core;
using Anch.IdentitySource;

namespace ExampleApp.Infrastructure.Services;

public class DomainObjectSaveStrategyServiceProxyBinder<TDomainObject>(IIdentityInfo<TDomainObject> identityInfo) : IServiceProxyBinder
{
    public Type GetTargetServiceType() =>
        typeof(DomainObjectSaveStrategy<,>).MakeGenericType(identityInfo.DomainObjectType, identityInfo.IdentityType);
}