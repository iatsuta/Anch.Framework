using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.DomainServices;

public interface IDomainSecurityService<TDomainObject>
{
    ISecurityProvider<TDomainObject> GetSecurityProvider(SecurityRule securityRule);
}
