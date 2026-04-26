using Anch.Core;
using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.GeneralPermission;

public class GeneralPermissionSystemFactory(IServiceProxyFactory serviceProxyFactory, PermissionBindingInfo bindingInfo)
    : IPermissionSystemFactory
{
    public IPermissionSystem Create(SecurityRuleCredential securityRuleCredential)
    {
        var permissionSystemType = typeof(GeneralPermissionSystem<>).MakeGenericType(bindingInfo.PermissionType);

        return serviceProxyFactory.Create<IPermissionSystem>(permissionSystemType, securityRuleCredential);
    }
}