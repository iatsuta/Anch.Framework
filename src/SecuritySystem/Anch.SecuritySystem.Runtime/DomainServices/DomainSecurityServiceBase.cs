using Anch.Core.DictionaryCache;
using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.DomainServices;

/// <summary>The base of the domain security services; caches the provider of each rule.</summary>
public abstract class DomainSecurityServiceBase<TDomainObject> : IDomainSecurityService<TDomainObject>
{
    private readonly IDictionaryCache<SecurityRule, ISecurityProvider<TDomainObject>> providersCache;

    protected DomainSecurityServiceBase() =>
        this.providersCache = new DictionaryCache<SecurityRule, ISecurityProvider<TDomainObject>>(securityRule =>
            this.CreateSecurityProvider(securityRule)
                .OverrideAccessDeniedResult(accessDeniedResult => accessDeniedResult with { SecurityRule = securityRule })).WithLock();

    protected abstract ISecurityProvider<TDomainObject> CreateSecurityProvider(SecurityRule securityRule);

    public ISecurityProvider<TDomainObject> GetSecurityProvider(SecurityRule securityRule) => this.providersCache[securityRule];
}