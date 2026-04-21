using CommonFramework;

using SecuritySystem.ExternalSystem;

namespace SecuritySystem.DiTests.Environment;

public class TestPermissionSystemFactory(IServiceProxyFactory serviceProxyFactory) : IPermissionSystemFactory
{
    public IPermissionSystem Create(SecurityRuleCredential securityRuleCredential) =>
        serviceProxyFactory.Create<IPermissionSystem, TestPermissionSystem>();
}