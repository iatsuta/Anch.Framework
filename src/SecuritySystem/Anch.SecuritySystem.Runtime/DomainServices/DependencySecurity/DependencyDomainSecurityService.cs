using Anch.GenericRepository;
using Anch.RelativePath;
using Anch.SecuritySystem.DomainServices.DependencySecurity._Base;
using Anch.SecuritySystem.Expanders;
using Anch.SecuritySystem.Providers;
using Anch.SecuritySystem.Providers.DependencySecurity;

namespace Anch.SecuritySystem.DomainServices.DependencySecurity;

public class DependencyDomainSecurityService<TDomainObject, TBaseDomainObject>(
    ISecurityRuleExpander securityRuleExpander,
    IDomainSecurityService<TBaseDomainObject> baseDomainSecurityService,
    IQueryableSource queryableSource,
    IRelativeDomainPathInfo<TDomainObject, TBaseDomainObject> relativeDomainPathInfo)
    : DependencyDomainSecurityServiceBase<TDomainObject, TBaseDomainObject>(securityRuleExpander, baseDomainSecurityService)
    where TBaseDomainObject : class
{
    protected override ISecurityProvider<TDomainObject> CreateDependencySecurityProvider(ISecurityProvider<TBaseDomainObject> baseProvider)
    {
        return new DependencySecurityProvider<TDomainObject, TBaseDomainObject>(baseProvider, relativeDomainPathInfo, queryableSource);
    }
}