using Anch.Core;
using Anch.IdentitySource;
using Anch.SecuritySystem.DomainServices.DependencySecurity._Base;
using Anch.SecuritySystem.Expanders;
using Anch.SecuritySystem.Providers;
using Anch.SecuritySystem.Providers.DependencySecurity;

namespace Anch.SecuritySystem.DomainServices.DependencySecurity;

public class UntypedDependencyDomainSecurityService<TDomainObject, TBaseDomainObject>(
    IServiceProxyFactory serviceProxyFactory,
    ISecurityRuleExpander securityRuleExpander,
    IDomainSecurityService<TBaseDomainObject> baseDomainSecurityService,
    IIdentityInfo<TDomainObject> domainIdentityInfo)
    : DependencyDomainSecurityServiceBase<TDomainObject, TBaseDomainObject>(
        securityRuleExpander,
        baseDomainSecurityService)
{
    protected override ISecurityProvider<TDomainObject> CreateDependencySecurityProvider(ISecurityProvider<TBaseDomainObject> baseProvider)
    {
        var securityProviderType = typeof(UntypedDependencySecurityProvider<,,>)
            .MakeGenericType(typeof(TDomainObject), typeof(TBaseDomainObject), domainIdentityInfo.IdentityType);

        return serviceProxyFactory.Create<ISecurityProvider<TDomainObject>>(securityProviderType, baseProvider);
    }
}