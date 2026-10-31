using Anch.GenericRepository;
using Anch.RelativePath;
using Anch.SecuritySystem.DomainServices.DependencySecurity._Base;
using Anch.SecuritySystem.Expanders;
using Anch.SecuritySystem.Providers;
using Anch.SecuritySystem.Providers.DependencySecurity;

namespace Anch.SecuritySystem.DomainServices.DependencySecurity;

/// <summary>Protects a type through a related base type: access to an object follows access to the related object.</summary>
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