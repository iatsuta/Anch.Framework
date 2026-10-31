using Anch.Core;
using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Creates the GeneralPermission system for a user credential.</summary>
public class GeneralPermissionSystemFactory(IServiceProxyFactory serviceProxyFactory, PermissionBindingInfo bindingInfo)
    : IPermissionSystemFactory
{
    public IPermissionSystem Create(SecurityRuleCredential securityRuleCredential)
    {
        var permissionSystemType = typeof(GeneralPermissionSystem<>).MakeGenericType(bindingInfo.PermissionType);

        return serviceProxyFactory.Create<IPermissionSystem>(permissionSystemType, securityRuleCredential);
    }
}