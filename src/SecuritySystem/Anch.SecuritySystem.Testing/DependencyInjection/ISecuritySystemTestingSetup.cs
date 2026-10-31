namespace Anch.SecuritySystem.Testing.DependencyInjection;

/// <summary>Fluent configuration of the testing helpers.</summary>
public interface ISecuritySystemTestingSetup
{
    /// <summary>Sets the type that runs test code in a scope.</summary>
    /// <param name="evaluatorType">The evaluator type.</param>
    ISecuritySystemTestingSetup SetEvaluator(Type evaluatorType);

    /// <summary>Sets the user that tests run as by default.</summary>
    /// <param name="getInfo">Creates the user description.</param>
    ISecuritySystemTestingSetup SetTestRootUserInfo(Func<IServiceProvider, TestRootUserInfo> getInfo);
}