using Xunit.Sdk;
using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonFrameworkExecutor(IXunitTestAssembly testAssembly, IServiceProvider serviceProvider) : XunitTestFrameworkExecutor(testAssembly)
{
    public override ValueTask RunTestCases(IReadOnlyCollection<IXunitTestCase> testCases, IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions,
        CancellationToken cancellationToken)
    {
        return base.RunTestCases(testCases, executionMessageSink, executionOptions, cancellationToken);
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer()
    {
        var baseDiscover = base.CreateDiscoverer();

        return baseDiscover;
    }

    //public CommonFrameworkExecutor(
    //    IXunitTestAssembly testAssembly,
    //    Assembly assembly)
    //    : base(testAssembly)
    //{
    //    this.assembly = assembly;

    //    try
    //    {
    //        FwServiceProvider = ReflectionUtils.GetImplementationInstanceOf<IFrameworkInitializer>(new AssemblyName(this.assembly.FullName!))
    //            ?.GetFrameworkServiceProvider();
    //    }
    //    catch (Exception e)
    //    {
    //        this.Aggregator.Add(e);
    //    }
    //}

    //public override async ValueTask RunTestCases(
    //    IReadOnlyCollection<IXunitTestCase> testCases,
    //    IMessageSink executionMessageSink,
    //    ITestFrameworkExecutionOptions executionOptions,
    //    CancellationToken cancellationToken)
    //{
    //    if (FwServiceProvider?.GetService<IAssemblyInitializeAndCleanup>() is { } init)
    //    {
    //        await this.Aggregator.RunAsync(async () => await init.EnvironmentInitializeAsync());
    //    }

    //    await XunitTestAssemblyRunner.Instance.Run(
    //        this.TestAssembly,
    //        testCases,
    //        executionMessageSink,
    //        executionOptions,
    //        cancellationToken);

    //    if (FwServiceProvider?.GetService<IAssemblyInitializeAndCleanup>() is { } cleanup)
    //    {
    //        await this.Aggregator.RunAsync(async () => await cleanup.EnvironmentCleanupAsync());
    //    }

    //    return;
    //}
}

public interface IDatabaseGenerator
{
    Task CreateSchemaAsync(CancellationToken cancellationToken = default);
}