using Anch.Core;

namespace Anch.SecuritySystem.Services;

/// <summary>Creates converters of principal entities into managed principal headers.</summary>
public class ManagedPrincipalHeaderConverterFactory<TPrincipal>(IServiceProxyFactory serviceProxyFactory)
    : IManagedPrincipalHeaderConverterFactory<TPrincipal>
{
    public IManagedPrincipalHeaderConverter<TPrincipal> Create(PermissionBindingInfo bindingInfo)
    {
        return serviceProxyFactory.Create<IManagedPrincipalHeaderConverter<TPrincipal>, ManagedPrincipalHeaderConverter<TPrincipal>>(
            Tuple.Create<PermissionBindingInfo?>(bindingInfo));
    }
}