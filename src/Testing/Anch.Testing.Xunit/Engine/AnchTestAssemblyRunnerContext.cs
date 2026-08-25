using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestAssemblyRunnerContext(
    IXunitTestAssembly testAssembly,
    IReadOnlyCollection<IXunitTestCase> testCases,
    IMessageSink executionMessageSink,
    ITestFrameworkExecutionOptions executionOptions,
    CancellationToken ct,
    AnchTestCollectionRunner commonTestCollectionRunner)
    : XunitTestAssemblyRunnerContext(testAssembly, testCases, executionMessageSink, executionOptions, ct)
{
    public new async ValueTask<RunSummary> RunTestCollection(
        IXunitTestCollection testCollection,
        IReadOnlyCollection<IXunitTestCase> testCases,
        ITestCaseOrderer testCaseOrderer)
    {
        return await commonTestCollectionRunner.Run(
            testCollection,
            testCases, this.ExplicitOption, this.MessageBus,
            this.Aggregator.Clone(), this.CancellationTokenSource,
            this.ParallelMode, this.Scheduler, this.AssemblyFixtureMappings);
    }
}
