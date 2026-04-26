using Anch.Core;
using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.DiTests.Environment;

public class TestPermissionSystemFactory(IServiceProxyFactory serviceProxyFactory) : IPermissionSystemFactory
{
    public IPermissionSystem Create(SecurityRuleCredential securityRuleCredential) =>
        serviceProxyFactory.Create<IPermissionSystem, TestPermissionSystem>();
}