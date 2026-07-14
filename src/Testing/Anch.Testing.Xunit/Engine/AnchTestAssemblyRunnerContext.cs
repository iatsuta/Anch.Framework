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
    private static readonly Func<XunitTestAssemblyRunnerBaseContext<IXunitTestAssembly, IXunitTestCase>, SemaphoreSlim?> GetParallelSemaphore =
        PrivateMemberAccessor.GetInstanceField<XunitTestAssemblyRunnerBaseContext<IXunitTestAssembly, IXunitTestCase>, SemaphoreSlim?>("parallelSemaphore");

    public new async ValueTask<RunSummary> RunTestCollection(
        IXunitTestCollection testCollection,
        IReadOnlyCollection<IXunitTestCase> testCases,
        ITestCaseOrderer testCaseOrderer)
    {
        var parallelSemaphore = GetParallelSemaphore(this);

        if (parallelSemaphore is not null)
            await parallelSemaphore.WaitAsync(this.CancellationTokenSource.Token);

        try
        {
            return await commonTestCollectionRunner.Run(
                testCollection,
                testCases, this.ExplicitOption, this.MessageBus,
                testCaseOrderer, this.Aggregator.Clone(), this.CancellationTokenSource, this.AssemblyFixtureMappings);
        }
        finally
        {
            parallelSemaphore?.Release();
        }
    }
}