namespace SecuritySystem.Testing.DependencyInjection;

public interface ISecuritySystemTestingSetup
{
    ISecuritySystemTestingSetup SetEvaluator(Type evaluatorType);

    ISecuritySystemTestingSetup SetTestRootUserInfo(Func<IServiceProvider, TestRootUserInfo> getInfo);
}