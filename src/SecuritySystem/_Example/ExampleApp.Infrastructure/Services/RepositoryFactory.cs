using Anch.Core;
using Anch.SecuritySystem;
using Anch.SecuritySystem.DomainServices;
using ExampleApp.Application;

namespace ExampleApp.Infrastructure.Services;

public class RepositoryFactory<TDomainObject>(
    IServiceProxyFactory serviceProxyFactory,
    IDomainSecurityService<TDomainObject> domainSecurityService)
    : IRepositoryFactory<TDomainObject>
    where TDomainObject : class
{
    public IRepository<TDomainObject> Create(SecurityRule securityRule) =>
        serviceProxyFactory.Create<IRepository<TDomainObject>, Repository<TDomainObject>>(
            domainSecurityService.GetSecurityProvider(securityRule));
}