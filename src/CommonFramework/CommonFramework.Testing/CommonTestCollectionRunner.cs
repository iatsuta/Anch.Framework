using Xunit.Internal;

using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonTestCollectionRunner(CommonTestClassRunner commonTestClassRunner) : XunitTestCollectionRunner
{
    protected override ValueTask<RunSummary> RunTestClass(XunitTestCollectionRunnerContext ctxt, IXunitTestClass? testClass, IReadOnlyCollection<IXunitTestCase> testCases)
    {
        Guard.ArgumentNotNull(ctxt);
        Guard.ArgumentNotNull(testCases);

        if (testClass is null)
            return new(XunitRunnerHelper.FailTestCases(
                ctxt.MessageBus,
                ctxt.CancellationTokenSource,
                testCases,
                "Test case '{0}' does not have an associated class and cannot be run by XunitTestClassRunner",
                sendTestClassMessages: true,
                sendTestMethodMessages: true
            ));

        return
            commonTestClassRunner.Run(
                testClass,
                testCases,
                ctxt.ExplicitOption,
                ctxt.MessageBus,
                ctxt.TestCaseOrderer,
                ctxt.Aggregator.Clone(),
                ctxt.CancellationTokenSource,
                ctxt.CollectionFixtureMappings
            );
    }
}