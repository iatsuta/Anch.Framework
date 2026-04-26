using System.Collections.Concurrent;
using Anch.Core;
using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Services;

public class PrincipalDataSecurityIdentityManager(IServiceProxyFactory serviceProxyFactory) : IPrincipalDataSecurityIdentityManager
{
    private readonly ConcurrentDictionary<Type, IPrincipalDataSecurityIdentityManager> cache = [];

    public TypedSecurityIdentity Extract(PrincipalData principalData)
    {
        return this.cache.GetOrAdd(principalData.PrincipalType, _ =>
        {
            var serviceType = typeof(PrincipalDataSecurityIdentityManager<>).MakeGenericType(principalData.PrincipalType);

            return serviceProxyFactory.Create<IPrincipalDataSecurityIdentityManager>(serviceType);
        }).Extract(principalData);
    }
}

public class PrincipalDataSecurityIdentityManager<TPrincipal>(ISecurityIdentityManager<TPrincipal> securityIdentityManager)
    : IPrincipalDataSecurityIdentityManager
{
    public TypedSecurityIdentity Extract(PrincipalData principalData)
    {
        var typedPrincipalData = (PrincipalData<TPrincipal>)principalData;

        return securityIdentityManager.GetIdentity(typedPrincipalData.Principal);
    }
}