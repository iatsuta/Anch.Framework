using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace CommonFramework.Testing.Engine;

public class CommonXunitTestAssemblyRunner(CommonTestCollectionRunner commonTestCollectionRunner) : XunitTestAssemblyRunnerBase<CommonXunitTestAssemblyRunnerContext, IXunitTestAssembly, IXunitTestCollection, IXunitTestCase>
{
    public async ValueTask<RunSummary> Run(
        IXunitTestAssembly testAssembly,
        IReadOnlyCollection<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions,
        CancellationToken cancellationToken)
    {
        Guard.ArgumentNotNull(testAssembly);
        Guard.ArgumentNotNull(testCases);
        Guard.ArgumentNotNull(executionMessageSink);
        Guard.ArgumentNotNull(executionOptions);

        await using var ctxt = new CommonXunitTestAssemblyRunnerContext(
            testAssembly,
            testCases,
            executionMessageSink,
            executionOptions,
            cancellationToken, commonTestCollectionRunner
        );

        await ctxt.InitializeAsync();

        return await this.Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTestCollection(CommonXunitTestAssemblyRunnerContext ctxt, IXunitTestCollection testCollection, IReadOnlyCollection<IXunitTestCase> testCases)
    {
        Guard.ArgumentNotNull(ctxt);
        Guard.ArgumentNotNull(testCollection);
        Guard.ArgumentNotNull(testCases);

        var testCaseOrderer = ctxt.AssemblyTestCaseOrderer ?? DefaultTestCaseOrderer.Instance;

        return ctxt.RunTestCollection(testCollection, testCases, testCaseOrderer);
    }
}