using Anch.Core;
using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.VirtualPermission;

/// <summary>Creates the VirtualPermission system for a user credential.</summary>
public class VirtualPermissionSystemFactory<TPermission>(IServiceProxyFactory serviceProxyFactory) : IPermissionSystemFactory
    where TPermission : class
{
    public IPermissionSystem Create(SecurityRuleCredential securityRuleCredential) =>

        serviceProxyFactory.Create<IPermissionSystem, VirtualPermissionSystem<TPermission>>(securityRuleCredential);
}